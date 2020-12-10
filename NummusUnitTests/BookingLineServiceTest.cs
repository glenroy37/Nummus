using System;
using System.Linq;
using Moq;
using Nummus.Data;
using Nummus.Exception;
using Nummus.Service;
using NUnit.Framework;

namespace NummusUnitTests {
    public class BookingLineServiceTest {
        private Mock<IAccountService> _accountService;
        private BookingLineService _bookingLineService;
        private NummusDbContext _nummusDbContext;
        private NummusUserService _nummusUserService;

        [SetUp]
        public void Setup() {
            _nummusDbContext = TestDbContextBuilder.InMemoryContext();
            _nummusUserService = TestUserInitialiser.InitialiseTestUserService(_nummusDbContext);
            _accountService = new Mock<IAccountService>();
            _bookingLineService = new BookingLineService(_nummusDbContext, _accountService.Object);
            
            var account = new Account("test-account", _nummusUserService.CurrentNummusUser);
            _nummusDbContext.Accounts.Add(account);
            _accountService.Setup(accountService => accountService.GetAccount(It.Is<int>(it => it == 1))).Returns(account);
        }

        [Test]
        public void TestCreateBookingLineValidAccountWithoutCategory() {
            var bookingDate = DateTime.Now;
            _bookingLineService.CreateBookingLine(1, new BookingLine {
                Amount = 1m,
                BookingTime = bookingDate,
                BookingText = "Gnampf"
            });

            var createdBookingLine = _nummusDbContext.BookingLines.FirstOrDefault(it => it.BookingText == "Gnampf");
           
            Assert.NotNull(createdBookingLine);
            Assert.AreEqual(1m, createdBookingLine.Amount);
            Assert.AreEqual(bookingDate, createdBookingLine.BookingTime);
        }

        [Test]
        public void TestCreateBookingLineInvalidAccount() {
            Assert.Throws<NummusInvalidAccountIdException>(() => {
                _bookingLineService.CreateBookingLine(-1, new BookingLine {
                    Amount = 1m,
                    BookingTime = DateTime.Now,
                    BookingText = "Gnampf"
                });
            });
        }
    }
}