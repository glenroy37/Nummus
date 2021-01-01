using Nummus.Data;

namespace Nummus.Service {
    public interface IBookingLineService {
        public void CreateBookingLine(int accountId, BookingLine bookingLine);
        public BookingLine[] GetBookingLinesPaged(int accountId, int size, int page);
        public int CountBookingLines(int accountId);
        public void SaveBookingLine(BookingLine bookingLine);
        public void DeleteBookingLine(BookingLine bookingLine);
    }
}