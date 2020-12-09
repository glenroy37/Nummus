using System.Linq;
using Nummus.Data;

namespace Nummus.Service {
    public class BookingLineService {
        private readonly NummusDbContext _nummusDbContext;

        public BookingLineService(NummusDbContext nummusDbContext) {
            _nummusDbContext = nummusDbContext;
        }

        public BookingLine[] GetBookingLinesPaged(Account account, int size, int page) {
            return account.BookingLines
                .OrderByDescending(it => it.BookingTime)
                .Skip(page * size)
                .Take(size)
                .ToArray();
        }

        public int CountBookingLines(Account account) {
            return account.BookingLines.Count;
        }
        
        public void SaveBookingLine(BookingLine bookingLine) {
            if (BookingLineExists(bookingLine.Id)) {
                _nummusDbContext.BookingLines.Add(bookingLine);
            } else {
                _nummusDbContext.BookingLines.Update(bookingLine);
            }
        }

        private bool BookingLineExists(long bookingLineId) {
            return _nummusDbContext.BookingLines
                .FirstOrDefault(it => it.Id == bookingLineId) != null;
        }
    }
}