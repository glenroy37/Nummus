using System;
using System.Collections.Generic;
using System.Linq;
using Nummus.Data;
using Nummus.Exception;

namespace Nummus.Service {
    public class AccountStatementService : IAccountStatementService {
        private readonly NummusDbContext _nummusDbContext;
        private readonly NummusUserService _nummusUserService;
        private readonly IAccountService _accountService;
        
        public AccountStatementService(NummusDbContext nummusDbContext, 
            NummusUserService nummusUserService, IAccountService accountService) {
            _nummusDbContext = nummusDbContext;
            _accountService = accountService;
            _nummusUserService = nummusUserService;
        }
        
        public DateTime? GeneretableStatement() {
            var now = DateTime.Now;

            if (!_nummusDbContext.Accounts
                .Any(it => it.NummusUser == _nummusUserService.CurrentNummusUser)) {
                return null;
            }
            
            if (!_nummusDbContext.AccountStatements
                .Any(it => it.Account.NummusUser == _nummusUserService.CurrentNummusUser)) {
                return now.AddMonths(-1);
            }

            var nextPossibleStatement = _nummusDbContext.AccountStatements
                .Where(it => it.Account.NummusUser == _nummusUserService.CurrentNummusUser)
                .OrderByDescending(it => it.BookingDate)
                .Select(it => it.BookingDate)
                .First()
                .AddMonths(1);

            if (nextPossibleStatement.Month == now.Month && nextPossibleStatement.Year == now.Year) {
                return null;
            }

            return nextPossibleStatement;
        }

        public void GenerateStatement() {
            var generatableStatementDateNullable = GeneretableStatement();

            if (generatableStatementDateNullable == null) {
                return;
            }

            var generatableStatementDate = generatableStatementDateNullable.Value;

            foreach (var account in _accountService.GetAllAccounts()) {
                var accountStatement = new AccountStatement {
                    Account = account,
                    BookingDate = generatableStatementDate
                };

                var lastAccountStatement = _nummusDbContext.AccountStatements
                    .Where(it => it.Account == account)
                    .OrderByDescending(it => it.BookingDate)
                    .FirstOrDefault();

                var closingSum = 0m;
                if (lastAccountStatement != null) {
                    var expectedLastStatementDate = generatableStatementDate.AddMonths(-1);
                    if (lastAccountStatement.BookingDate.Month != expectedLastStatementDate.Month ||
                        lastAccountStatement.BookingDate.Year != expectedLastStatementDate.Year) {
                        throw new NummusInBetweenAccountStatementMissingException();
                    }
                    closingSum = lastAccountStatement.ClosingSum;
                }

                var addedBookingLines = new List<BookingLine>();
                foreach (var bookingLine in account.BookingLines
                    .Where(it => it.AccountStatement == null)
                    .Where(it => it.Account.NummusUser == _nummusUserService.CurrentNummusUser)
                    .Where(it => it.BookingTime.Month == generatableStatementDate.Month)
                    .Where(it => it.BookingTime.Year == generatableStatementDate.Year)) {
                    bookingLine.AccountStatement = accountStatement;
                    addedBookingLines.Add(bookingLine);
                    closingSum += bookingLine.Amount;
                }
                accountStatement.ClosingSum = closingSum;
                
                _nummusDbContext.AccountStatements.Add(accountStatement);
                _nummusDbContext.BookingLines.UpdateRange(addedBookingLines);
            }
            _nummusDbContext.SaveChanges();
        }

        public AccountStatement[] GetLatestStatements() {
            var latestStatementDate = _nummusDbContext.AccountStatements
                .Where(it => it.Account.NummusUser == _nummusUserService.CurrentNummusUser)
                .OrderByDescending(it => it.BookingDate)
                .FirstOrDefault();
            
            return latestStatementDate != null ?
                GetStatementsOf(latestStatementDate.BookingDate) :
                Array.Empty<AccountStatement>();
        }

        public AccountStatement[] GetStatementsOf(DateTime month) {
            return _nummusDbContext.AccountStatements
                .Where(it => it.Account.NummusUser == _nummusUserService.CurrentNummusUser)
                .Where(it => it.BookingDate.Month == month.Month)
                .Where(it => it.BookingDate.Year == month.Year)
                .ToArray();
        }
    }
}