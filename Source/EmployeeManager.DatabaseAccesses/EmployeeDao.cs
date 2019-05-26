using Dapper.FastCrud;

namespace AdventureWorks.EmployeeManager.DatabaseAccesses
{
    public class EmployeeDao
    {
        private readonly ITransactionContext _transactionContext;

        public EmployeeDao(ITransactionContext transactionContext)
        {
            _transactionContext = transactionContext;
        }

        public virtual Employee FindById(int businessEntityID)
            => _transactionContext.Connection.Get(new Employee {BusinessEntityID = businessEntityID});

        public virtual void Update(Employee employee)
            => _transactionContext.Connection.Update(employee);

        public virtual void Insert(Employee employee)
            => _transactionContext.Connection.Insert(employee);
    }
}
