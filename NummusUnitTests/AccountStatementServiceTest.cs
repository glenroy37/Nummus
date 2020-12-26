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
        public void TestGeneratableStatementsNoPreviousStatements() {
            CreateStatementOtherUser();
            var generatableStatements = _accountStatementService.GeneretableStatements();
            Assert.AreEqual(1, generatableStatements.Length);
            Assert.AreEqual(DateTime.Now.Month, generatableStatements[0].Item1);
            Assert.AreEqual(DateTime.Now.Year, generatableStatements[0].Item2);
        }

        [Test]
        public void TestGeneratableStatementsPreviousLastMonth() {
            var account = CreateAccount();
            CreateStatement(DateTime.Now.AddMonths(-1), account);
            var generatableStatements = _accountStatementService.GeneretableStatements();
            Assert.AreEqual(1, generatableStatements.Length);
            Assert.AreEqual(DateTime.Now.Month, generatableStatements[0].Item1);
            Assert.AreEqual(DateTime.Now.Year, generatableStatements[0].Item2);
        }

        [Test]
        public void TestGeneratableStatementsPrevious14MonthsAgo() {
            var account = CreateAccount();
            var statementDate = DateTime.Now.AddMonths(-14);
            CreateStatement(statementDate, account);
            var generatableStatements = _accountStatementService.GeneretableStatements();
            Assert.AreEqual(14, generatableStatements.Length);
        }

        [Test]
        public void GenerateAccountStatementNoBookingLines() {
            var account = CreateAccount();
            _accountServiceMock.Setup(it =>
                it.GetAllAccounts()).Returns(new[] {account});
            _accountStatementService.GenerateStatement(DateTime.Now.Month, DateTime.Now.Year);
            var statement = _nummusDbContext.AccountStatements.First();
            Assert.AreEqual(0m, statement.ClosingSum);
            Assert.AreEqual(DateTime.Now.Month, statement.BookingDate.Month);
            Assert.AreEqual(DateTime.Now.Year, statement.BookingDate.Year);
            Assert.AreEqual(account.Name, statement.Account.Name);
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
    }
}