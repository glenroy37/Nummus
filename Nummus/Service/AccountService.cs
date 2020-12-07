using System.Collections.Generic;
using System.Linq;
using Nummus.Data;

namespace Nummus.Service {
    public class AccountService {

        private readonly NummusDbContext _nummusDbContext;
        private readonly NummusUserService _nummusUserService;

        public AccountService(NummusDbContext nummusDbContext, NummusUserService nummusUserService) {
            this._nummusDbContext = nummusDbContext;
            this._nummusUserService = nummusUserService;
        }

        public HashSet<Account> GetAllAccountsAsync() {
            return _nummusDbContext.Accounts
                .Where(account => account.NummusUser.Id == _nummusUserService.CurrentNummusUser.Id)
                .ToHashSet();
        }

        public void CreateAccount(string name) {
            var account = new Account(name, _nummusUserService.CurrentNummusUser);
            _nummusDbContext.Accounts.Add(account);
        }
    }
}
