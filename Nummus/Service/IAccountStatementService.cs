using System;
using Nummus.Data;

namespace Nummus.Service {
    public interface IAccountStatementService {
        public (int, int)[] GeneretableStatements();
        public void GenerateStatement(int month, int year);
        public AccountStatement[] GetAllStatementsOfYear(int year);
    }
}