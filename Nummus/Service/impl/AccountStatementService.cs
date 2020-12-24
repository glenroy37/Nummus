using System;
using System.Collections.Generic;
using System.Linq;
using Nummus.Data;

namespace Nummus.Service {
    public class AccountStatementService : IAccountStatementService {
        private readonly NummusDbContext _nummusDbContext;
        private readonly IAccountService _accountService;
        
        public AccountStatementService(NummusDbContext nummusDbContext, IAccountService accountService) {
            _nummusDbContext = nummusDbContext;
            _accountService = accountService;
        }
        
        public (int, int)[] GeneretableStatements() {
            var now = DateTime.Now;

            if (!_nummusDbContext.AccountStatements.Any()) {
                return new (int, int)[] {
                    new (now.Month, now.Year)
                };
            }

            var lastAccountStatementDate = _nummusDbContext.AccountStatements
                .OrderByDescending(it => it.BookingDate)
                .Select(it => it.BookingDate)
                .FirstOrDefault();

            var tuples = new HashSet<(int, int)>();
            for (var year = lastAccountStatementDate.Year; year <= now.Year+1; year++) {
                for (var month = (year == lastAccountStatementDate.Year) ? lastAccountStatementDate.Month : 1; 
                    month <= 12; month++) {
                    tuples.Add(new (month, year));
                }
            }
            return tuples.ToArray();
        }

        public void GenerateStatement(int month, int year) {
            foreach (var account in _accountService.GetAllAccounts()) {
                var accountStatement = new AccountStatement {
                    Account = account,
                    BookingDate =  new DateTime(year, month, 1).AddMonths(1).AddDays(-1)
                };

                var addedBookingLines = new List<BookingLine>();
                var closingSum = 0m;
                foreach (var bookingLine in account.BookingLines
                    .Where(it => it.AccountStatement == null)
                    .Where(it => it.BookingTime.Month == month)
                    .Where(it => it.BookingTime.Year == year)) {
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

        public AccountStatement[] GetAllStatementsOfYear(int year) {
            return _nummusDbContext.AccountStatements
                .Where(it => it.BookingDate.Year == year)
                .ToArray();
        }
    }
}