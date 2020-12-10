using Nummus.Data;

namespace Nummus.Service {
    public interface IAccountService {
        public Account[] GetAllAccounts();
        public Account GetAccount(int id);
        public decimal GetCurrentAccountBalance(decimal accountId);
        public void CreateAccount(string name);
    }
}