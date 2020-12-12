using System;
using System.Linq;
using Nummus.Data;
using Nummus.Exception;

namespace Nummus.Service {
    public class BookingLineService : IBookingLineService {
        private readonly NummusDbContext _nummusDbContext;
        private readonly IAccountService _accountService;

        public BookingLineService(NummusDbContext nummusDbContext, 
            IAccountService accountService) {
            _nummusDbContext = nummusDbContext;
            _accountService = accountService;
        }

        public BookingLine[] GetBookingLinesPaged(int accountId, int size, int page) {
            var account = _accountService.GetAccount(accountId);
            if (account == null) {
                throw new NummusInvalidAccountIdException();
            }
            
            return _nummusDbContext
                .BookingLines
                .Where(it => it.Account == account)
                .OrderByDescending(it => it.BookingTime)
                .Skip(page * size)
                .Take(size)
                .ToArray();
        }

        public int CountBookingLines(int accountId) {
            var account = _accountService.GetAccount(accountId);
            if (account == null) {
                throw new NummusInvalidAccountIdException();
            }
            
            return account.BookingLines.Count;
        }

        public void CreateBookingLine(int accountId, BookingLine bookingLine) {
            var account = _accountService.GetAccount(accountId);
            if (account == null) {
                throw new NummusInvalidAccountIdException();
            }

            bookingLine.Account = account;
            _nummusDbContext.Add(bookingLine);
            _nummusDbContext.SaveChanges();
        }
    }
}