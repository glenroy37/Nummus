using System;
using Nummus.Data;

namespace Nummus.Service {
    public interface IAccountStatementService {
        public DateTime? GeneretableStatement();
        public void GenerateStatement();
        public AccountStatement[] GetAllStatements();
        public AccountStatement[] GetLatestStatements();
        public AccountStatement[] GetStatementsOf(int month, int year);
    }
}