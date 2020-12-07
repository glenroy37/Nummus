using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Nummus.Data;
using Nummus.Service;

namespace NummusUnitTests {
    public class TestUserInitialiser {
        public static NummusUserService InitialiseTestUserService(NummusDbContext nummusDbContext) {
            const string userEmail = "valid@test.user";
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext.User.Identity.Name).Returns(userEmail);
            nummusDbContext.Users.Add(new IdentityUser(userEmail));
            return new NummusUserService(mockHttpContextAccessor.Object, nummusDbContext);
        }
    }
}