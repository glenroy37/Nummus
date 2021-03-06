using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                .Include("AccountStatement")
                .ToArray();
        }

        public int CountBookingLines(int accountId) {
            return _nummusDbContext.BookingLines
                .Count(it => it.Account.Id == accountId);
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

        public void SaveBookingLine(BookingLine bookingLine) {
            if (bookingLine.AccountStatement != null) {
                throw new NummusTriedToModifyUnmodifiableBookingLineException();
            }
            _nummusDbContext.BookingLines.Update(bookingLine);
            _nummusDbContext.SaveChanges();
        }

        public void DeleteBookingLine(BookingLine bookingLine) {
            if (bookingLine.AccountStatement != null) {
                throw new NummusTriedToModifyUnmodifiableBookingLineException();
            }
            _nummusDbContext.BookingLines.Remove(bookingLine);
            _nummusDbContext.SaveChanges();
        }
    }
}