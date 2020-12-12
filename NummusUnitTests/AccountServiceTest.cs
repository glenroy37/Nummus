using System;
using System.Linq;
using Nummus.Data;
using Nummus.Exception;
using Nummus.Service;
using NUnit.Framework;

namespace NummusUnitTests {
    public class AccountServiceTest {
        private AccountService _accountService;
        private NummusDbContext _nummusDbContext;
        private NummusUserService _nummusUserService;

        [SetUp]
        public void Setup() {
            _nummusDbContext = TestDbContextBuilder.InMemoryContext();
            _nummusUserService = TestUserInitialiser.InitialiseTestUserService(_nummusDbContext);
            _accountService = new AccountService(_nummusDbContext, _nummusUserService);
        }

        [Test]
        public void CreatNewAccount() {
            const string accountName = "new-account";
            _accountService.CreateAccount(accountName);

            var result = _nummusDbContext.Accounts
                .FirstOrDefault(it => it.Name == accountName);

            Assert.NotNull(result);
            Assert.NotNull(result.Id);
            Assert.AreEqual(accountName, result.Name);
        }

        [Test]
        public void CreateExistingAccountShouldFail() {
            const string accountName = "new-account";
            _accountService.CreateAccount(accountName);
            Assert.Throws<NummusAccountAlreadyExistsException>(() => _accountService.CreateAccount(accountName));
        }

        [Test]
        public void GetAllAccounts() {
            const string accountName1 = "new-account-1";
            _accountService.CreateAccount(accountName1);
            const string accountName2 = "new-account-2";
            _accountService.CreateAccount(accountName2);

            var result = _accountService.GetAllAccounts();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(accountName1, result[0].Name);
            Assert.AreEqual(accountName2, result[1].Name);
        }

        [Test]
        public void GetAccountBalanceNoStatementNoBookingLine() {
            var account = CreateAccountNoStatementNoBookingLine("new-account");
            Assert.AreEqual(decimal.Zero, _accountService.GetCurrentAccountBalance(account.Id));
        }

        [Test]
        public void GetAccountBalanceNoStatementsNewBookingLines() {
            const decimal newBookingLine1Amount = 47.11m;
            const decimal newBookingLine2Amount = -1.23m;
            const decimal shouldntInclude1 = 2m;
            const decimal shouldntInclude2 = 2m;
            var account = CreateAccountNoStatementsNewBookingLines("new-account", newBookingLine1Amount,
                newBookingLine2Amount);
            CreateAccountNoStatementsNewBookingLines("shouldnt-include", shouldntInclude1, 
                shouldntInclude2);
            Assert.AreEqual(newBookingLine1Amount + newBookingLine2Amount,
                _accountService.GetCurrentAccountBalance(account.Id));
        }

        [Test]
        public void GetAccountBalanceWithStatementsNoNewBookingLines() {
            const decimal balance = 123.45m;
            const decimal shouldntInclude = 1m;
            var account = CreateAccountWithStatementsNoNewBookingLine("new-account", balance);
            CreateAccountWithStatementsNoNewBookingLine("shouldnt-include", shouldntInclude);
            Assert.AreEqual(balance, _accountService.GetCurrentAccountBalance(account.Id));
        }

        [Test]
        public void GetAccountBalanceWithStatementsAndNewBookingLines() {
            const decimal balance = 123.45m;
            const decimal newBookingLine1Amount = 47.11m;
            const decimal newBookingLine2Amount = -1.23m;
            const decimal shouldntIncludeBalance = 1m;
            const decimal shouldntIncludeBookingLine1 = 1m;
            const decimal shouldntIncludeBookingLine2 = 1m;
            var account = CreateAccountWithStatementsAndNewBookingLines("new-account", balance,
                newBookingLine1Amount, newBookingLine2Amount);
            CreateAccountWithStatementsAndNewBookingLines("shouldnt-include", shouldntIncludeBalance,
                shouldntIncludeBookingLine1, shouldntIncludeBookingLine2);
            Assert.AreEqual(balance + newBookingLine1Amount + newBookingLine2Amount,
                _accountService.GetCurrentAccountBalance(account.Id));

        }

        private Account CreateAccountNoStatementNoBookingLine(string accountName) {
            return _nummusDbContext.Accounts.Add(new Account(accountName, _nummusUserService.CurrentNummusUser)).Entity;
        }

        private Account CreateAccountWithStatementsNoNewBookingLine(string accountName, decimal lastStatementSum) {
            var account = new Account(accountName, _nummusUserService.CurrentNummusUser);
            _nummusDbContext.Accounts.Add(account);
            _nummusDbContext.SaveChanges();
            account = _nummusDbContext.Accounts.FirstOrDefault(it => it.Name == accountName);

            var accountStatementOld = new AccountStatement {
                BookingDate = new DateTime(2020, 10, 31), 
                ClosingSum = decimal.One, 
                Account = account
            };
            _nummusDbContext.AccountStatements.Add(accountStatementOld);
            
            var accountStatementNew = new AccountStatement {
                BookingDate = new DateTime(2020, 11, 30),
                ClosingSum = lastStatementSum,
                Account = account
            };
            _nummusDbContext.AccountStatements.Add(accountStatementNew);
            _nummusDbContext.SaveChanges();
            
            var closedBookingLine1 = new BookingLine {
                BookingText = "Gnampf",
                Amount = decimal.One,
                BookingTime = new DateTime(2020, 10, 30),
                Account = account,
                AccountStatement = accountStatementOld
            };
            _nummusDbContext.BookingLines.Add(closedBookingLine1);

            var closedBookingLine2 = new BookingLine {
                BookingText = "Gnampf",
                Amount = decimal.One,
                BookingTime = new DateTime(2020, 11, 29),
                Account = account,
                AccountStatement = accountStatementNew
            };
            _nummusDbContext.BookingLines.Add(closedBookingLine2);

            _nummusDbContext.SaveChanges();
            return account;
        }

        private Account CreateAccountWithStatementsAndNewBookingLines(string accountName, decimal lastStatementSum,
            decimal bookingLine1Amount, decimal bookingLine2Amount) {
            var account = CreateAccountWithStatementsNoNewBookingLine(accountName, lastStatementSum);
            var openBookingLine1 = new BookingLine {
                BookingText = "Gnampf",
                Amount = bookingLine1Amount,
                BookingTime = new DateTime(2020, 12, 06),
                Account = account
            };
            _nummusDbContext.BookingLines.Add(openBookingLine1);
            
            var openBookingLine2 = new BookingLine {
                BookingText = "Gnampf",
                Amount = bookingLine2Amount,
                BookingTime = new DateTime(2020, 12, 07),
                Account = account
            };
            _nummusDbContext.BookingLines.Add(openBookingLine2);
            
            _nummusDbContext.SaveChanges();
            return account;
        }
        
        private Account CreateAccountNoStatementsNewBookingLines(string accountName, decimal bookingLine1Amount,
            decimal bookingLine2Amount) {
            var account = CreateAccountNoStatementNoBookingLine(accountName);
            var openBookingLine1 = new BookingLine {
                BookingText = "Gnampf",
                Amount = bookingLine1Amount,
                BookingTime = new DateTime(2020, 12, 06),
                Account = account
            };
            _nummusDbContext.BookingLines.Add(openBookingLine1);
            
            var openBookingLine2 = new BookingLine {
                BookingText = "Gnampf",
                Amount = bookingLine2Amount,
                BookingTime = new DateTime(2020, 12, 07),
                Account = account
            };
            _nummusDbContext.BookingLines.Add(openBookingLine2);
            
            _nummusDbContext.SaveChanges();
            return account;
        }
    }
}