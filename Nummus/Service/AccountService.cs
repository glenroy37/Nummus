using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nummus.Data;
using Nummus.Exception;

namespace Nummus.Service {
    public class AccountService {

        private readonly NummusDbContext _nummusDbContext;
        private readonly NummusUserService _nummusUserService;

        public AccountService(NummusDbContext nummusDbContext, NummusUserService nummusUserService) {
            this._nummusDbContext = nummusDbContext;
            this._nummusUserService = nummusUserService;
        }

        public Account[] GetAllAccounts() {
            return _nummusDbContext.Accounts
                .Where(account => account.NummusUser.Id == _nummusUserService.CurrentNummusUser.Id)
                .ToArray();
        }

        public decimal GetCurrentAccountBalance(Account account) {
            var lastClosingSum = _nummusDbContext.AccountStatements
                .Where(statement => statement.Account == account)
                .OrderByDescending(statement => statement.BookingDate)
                .Select(statement => statement.ClosingSum)
                .FirstOrDefault();

            var bookingsSinceLastStatement = _nummusDbContext.BookingLines
                .Where(bookingLine => bookingLine.AccountStatement == null)
                .Select(bookingLine => bookingLine.Amount)
                .ToHashSet();

            return lastClosingSum + bookingsSinceLastStatement.Sum();
        }

        public void CreateAccount(string name) {
            var existingAccount = _nummusDbContext.Accounts.FirstOrDefault(it => it.Name == name);
            if (existingAccount != null) {
                throw new NummusAccountAlreadyExistsException("An account with that name already exists!");
            }

            var account = new Account(name, _nummusUserService.CurrentNummusUser);
            _nummusDbContext.Accounts.Add(account);
            _nummusDbContext.SaveChanges();
        }
    }
}
