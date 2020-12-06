using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nummus.Data;

namespace Nummus.Service {
    public class NummusUserService {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDbContext applicationDbContext;

        public NummusUserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext) {
            this.httpContextAccessor = httpContextAccessor;
            this.applicationDbContext = applicationDbContext;
        }

        public NummusUser CurrentNummusUser {
            get {
                var userEmail = httpContextAccessor.HttpContext.User.Identity.Name;
                var nummusUser = applicationDbContext.NummusUsers.FirstOrDefault(nummusUser =>
                    nummusUser.UserEmail == userEmail
                );
                if (nummusUser != null) {
                    return nummusUser;
                }
                CreateNewNummusUser(userEmail);
                return applicationDbContext.NummusUsers.First(nummusUser =>
                    nummusUser.UserEmail == userEmail
                );
            }
        }

        private void CreateNewNummusUser(string userEmail) {
            applicationDbContext.Add(new NummusUser(userEmail));
            applicationDbContext.SaveChanges();
        }
    }
}
