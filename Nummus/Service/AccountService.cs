using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nummus.Data;

namespace Nummus.Service {
    public class AccountService {

        private readonly ApplicationDbContext applicationDbContext;
        private readonly NummusUserService nummusUserService;

        public AccountService(ApplicationDbContext applicationDbContext, NummusUserService nummusUserService) {
            this.applicationDbContext = applicationDbContext;
            this.nummusUserService = nummusUserService;
        }

        public HashSet<Account> GetAllAccountsAsync() {
            return applicationDbContext.Accounts
                .Where(account => account.NummusUser.Id == nummusUserService.CurrentNummusUser.Id)
                .ToHashSet();

        }

        public void CreateAccount(string name) {
            Account account = new Account(name, nummusUserService.CurrentNummusUser);
            applicationDbContext.Accounts.Add(account);
        }
    }
}
