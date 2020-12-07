using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nummus.Data;

namespace Nummus.Service {
    public class NummusUserService {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NummusDbContext _nummusDbContext;

        public NummusUserService(IHttpContextAccessor httpContextAccessor, NummusDbContext nummusDbContext) {
            this._httpContextAccessor = httpContextAccessor;
            this._nummusDbContext = nummusDbContext;
        }
        
        public NummusUser CurrentNummusUser {
            get {
                var userEmail = _httpContextAccessor?.HttpContext?.User.Identity?.Name;
                if (userEmail == null) {
                    return null;
                }
                var nummusUser = _nummusDbContext.NummusUsers.FirstOrDefault(it =>
                    it.UserEmail == userEmail
                );
                if (nummusUser != null) {
                    return nummusUser;
                }
                CreateNewNummusUser(userEmail);
                return _nummusDbContext.NummusUsers.First(it =>
                    it.UserEmail == userEmail
                );
            }
        }

        private void CreateNewNummusUser(string userEmail) {
            _nummusDbContext.Add(new NummusUser(userEmail));
            _nummusDbContext.SaveChanges();
        }
    }
}
