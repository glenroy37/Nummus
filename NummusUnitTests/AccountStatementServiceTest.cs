using System;
using System.Linq;
using Moq;
using Nummus.Data;
using Nummus.Service;
using NUnit.Framework;

namespace NummusUnitTests {
    public class AccountStatementServiceTest {
        private NummusDbContext _nummusDbContext;
        private NummusUserService _nummusUserService;
        private Mock<IAccountService> _accountServiceMock;
        private AccountStatementService _accountStatementService;

        [SetUp]
        public void SetUp() {
            _nummusDbContext = TestDbContextBuilder.InMemoryContext();
            _nummusUserService = TestUserInitialiser.InitialiseTestUserService(_nummusDbContext);
            _accountServiceMock = new Mock<IAccountService>();
            _accountStatementService = new AccountStatementService(_nummusDbContext,
                _nummusUserService, _accountServiceMock.Object);
        }

        [Test]
        public void TestNoStatementsGeneratableIfNoAccountsExist() {
            var generatableStatement = _accountStatementService.GeneretableStatement();
            Assert.IsNull(generatableStatement);
        }

        [Test]
        public void TestGeneratableStatementsNoPreviousStatements() {
            CreateStatementOtherUser();
            CreateAccount();
            var generatableStatement = _accountStatementService.GeneretableStatement();
            Assert.NotNull(generatableStatement);
            Assert.AreEqual(DateTime.Now.Month - 1, generatableStatement.Value.Month);
            Assert.AreEqual(DateTime.Now.Year, generatableStatement.Value.Year);
        }

        [Test]
        public void TestGeneratableStatementsPreviousLastMonth() {
            var account = CreateAccount();
            CreateStatement(DateTime.Now.AddMonths(-1), account);
            var generatableStatement = _accountStatementService.GeneretableStatement();
            Assert.Null(generatableStatement);
        }

        [Test]
        public void TestGeneratableStatementTwoMonthsAgo() {
            var account = CreateAccount();
            CreateStatement(DateTime.Now.AddMonths(-2), account);
            var generatableStatement = _accountStatementService.GeneretableStatement();
            Assert.NotNull(generatableStatement);
            Assert.AreEqual(DateTime.Now.Month - 1, generatableStatement.Value.Month);
            Assert.AreEqual(DateTime.Now.Year, generatableStatement.Value.Year);
        }

        [Test]
        public void TestGeneratableStatementsPrevious14MonthsAgo() {
            var account = CreateAccount();
            var statementDate = DateTime.Now.AddMonths(-14);
            CreateStatement(statementDate, account);
            var nextStatement = statementDate.AddMonths(1);
            var generatableStatement = _accountStatementService.GeneretableStatement();
            Assert.NotNull(generatableStatement);
            Assert.AreEqual(nextStatement.Month, generatableStatement.Value.Month);
            Assert.AreEqual(nextStatement.Year, generatableStatement.Value.Year);
        }

        [Test]
        public void GenerateAccountStatementNoBookingLines() {
            var account = CreateAccount();
            _accountServiceMock.Setup(it =>
                it.GetAllAccounts()).Returns(new[] {account});
            _accountStatementService.GenerateStatement();
            var statement = _nummusDbContext.AccountStatements.First();
            Assert.AreEqual(0m, statement.ClosingSum);
            Assert.AreEqual(DateTime.Now.Month - 1, statement.BookingDate.Month);
            Assert.AreEqual(DateTime.Now.Year, statement.BookingDate.Year);
            Assert.AreEqual(account.Name, statement.Account.Name);
        }

        [Test]
        public void GenerateAccountStatementWithBookingLines() {
            var account = CreateAccount();
            _accountServiceMock.Setup(it =>
                it.GetAllAccounts()).Returns(new[] {account});
            var bookingLineLastMonth = CreateBookingLine(DateTime.Now.AddMonths(-1), account); 
            CreateBookingLine(DateTime.Now, account);
            
            _accountStatementService.GenerateStatement();

            var statement = _nummusDbContext.AccountStatements.FirstOrDefault();
            Assert.AreEqual(1, statement.BookingLines.Count);
            Assert.AreEqual(bookingLineLastMonth.Id, statement.BookingLines.First().Id);
        }

        private void CreateStatementOtherUser() {
            var account = new Account("test-someone-else",
                TestUserInitialiser.AddSecondUser(_nummusDbContext));
            _nummusDbContext.Accounts.Add(account);
            var accountStatement = new AccountStatement {
                Account = account,
                BookingDate = DateTime.Now,
            };
            _nummusDbContext.AccountStatements.Add(accountStatement);
            _nummusDbContext.SaveChanges();
        }

        private Account CreateAccount() {
            var account = new Account("test-account", _nummusUserService.CurrentNummusUser);
            _nummusDbContext.Accounts.Add(account);
            _nummusDbContext.SaveChanges();
            return account;
        }

        private AccountStatement CreateStatement(DateTime dateTime, Account account) {
            var accountStatement = new AccountStatement {
                Account = account,
                BookingDate = dateTime,
            };
            _nummusDbContext.AccountStatements.Add(accountStatement);
            _nummusDbContext.SaveChanges();
            return accountStatement;
        }

        private BookingLine CreateBookingLine(DateTime dateTime, Account account) {
            var bookingLine = new BookingLine() {
                Account = account,
                BookingTime = dateTime,
                BookingText = "Gnampfiastic"
            };
            _nummusDbContext.BookingLines.Add(bookingLine);
            _nummusDbContext.SaveChanges();
            return bookingLine;
        }
    }
}