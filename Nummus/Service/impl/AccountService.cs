using System.Linq;
using Nummus.Data;
using Nummus.Exception;

namespace Nummus.Service {
    public class AccountService : IAccountService {

        private readonly NummusDbContext _nummusDbContext;
        private readonly NummusUserService _nummusUserService;

        public AccountService(NummusDbContext nummusDbContext, NummusUserService nummusUserService) {
            _nummusDbContext = nummusDbContext;
            _nummusUserService = nummusUserService;
        }

        public Account[] GetAllAccounts() {
            return _nummusDbContext.Accounts
                .Where(account => account.NummusUser.Id == _nummusUserService.CurrentNummusUser.Id)
                .ToArray();
        }

        public Account GetAccount(int id) {
            return _nummusDbContext.Accounts
                .FirstOrDefault(it => it.Id == id);
        }

        public decimal GetCurrentAccountBalance(decimal accountId) {
            var lastClosingSum = _nummusDbContext.AccountStatements
                .Where(statement => statement.Account.Id == accountId)
                .OrderByDescending(statement => statement.BookingDate)
                .Select(statement => statement.ClosingSum)
                .FirstOrDefault();

            var bookingsSinceLastStatement = _nummusDbContext.BookingLines
                .Where(bookingLine => bookingLine.Account.Id == accountId)
                .Where(bookingLine => bookingLine.AccountStatement == null)
                .Select(bookingLine => bookingLine.Amount)
                .ToList();

            return lastClosingSum + bookingsSinceLastStatement.Sum();
        }

        public void CreateAccount(string name) {
            var existingAccount = _nummusDbContext.Accounts.FirstOrDefault(it => it.Name == name);
            if (existingAccount != null) {
                throw new NummusAccountAlreadyExistsException();
            }

            var account = new Account(name, _nummusUserService.CurrentNummusUser);
            _nummusDbContext.Accounts.Add(account);
            _nummusDbContext.SaveChanges();
        }
    }
}
