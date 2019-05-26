using System.Collections.Generic;
using System.Data;
using Dapper.FastCrud;

namespace AdventureWorks.EmployeeManager.DatabaseAccesses
{
    public class ManagedEmployeeDao
    {
        private readonly ITransactionContext _transactionContext;

        private IDbConnection Connection => _transactionContext.Connection;

        public ManagedEmployeeDao(ITransactionContext transactionContext)
        {
            _transactionContext = transactionContext;
        }

        public IEnumerable<ManagedEmployee> GetManagedEmployees() => Connection.Find<ManagedEmployee>();
    }
}
