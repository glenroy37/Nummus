using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Moq;
using Nummus.Data;
using Nummus.Service;
using NUnit.Framework;

namespace NummusUnitTests {
    public class NummusUserServiceTest {
        private NummusUserService _nummusUserService;
        private NummusDbContext _nummusDbContext;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        [SetUp]
        public void Setup() {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _nummusDbContext = TestDbContextBuilder.InMemoryContext();
            _nummusUserService = new NummusUserService(_mockHttpContextAccessor.Object, _nummusDbContext);
        }

        [Test]
        public void CurrentNummusUserReturnsExistingUserWhenLoggedIn() {
            const string userEmail = "valid@test.user";
            var nummusDbUser = new NummusUser(userEmail) {Id = 4711};

            _mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns(userEmail);
            _nummusDbContext.NummusUsers.Add(nummusDbUser);

            var result = _nummusUserService.CurrentNummusUser;

            Assert.AreEqual(nummusDbUser.Id, result.Id);
            Assert.AreEqual(nummusDbUser.UserEmail, result.UserEmail);
        }
        
        [Test]
        public void CurrentNummusUserCreatesNewUserWhenItDoesntExist() {
            const string userEmail = "valid@test.user";

            _mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns(userEmail);
            var result = _nummusUserService.CurrentNummusUser;
            var dbUser = _nummusDbContext.NummusUsers.FirstOrDefault(it => it.UserEmail.Equals(userEmail));

            Assert.NotNull(dbUser);
            Assert.AreEqual(result.Id, dbUser.Id);
            Assert.AreEqual(userEmail, dbUser.UserEmail);
            Assert.AreEqual(userEmail, result.UserEmail);
        }

        [Test]
        public void ReturnNullIfNotLoggedIn() {
            var result = _nummusUserService.CurrentNummusUser;
            Assert.Null(result);
        }
    }
}