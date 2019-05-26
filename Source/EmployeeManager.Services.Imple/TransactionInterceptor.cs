﻿using AdventureWorks.EmployeeManager.DatabaseAccesses;
using Castle.DynamicProxy;

namespace AdventureWorks.EmployeeManager.Services.Imple
{
    public class TransactionInterceptor : IInterceptor
    {
        private readonly ITransactionContext _transactionContext;

        public TransactionInterceptor(ITransactionContext transactionContext)
        {
            _transactionContext = transactionContext;
        }

        public void Intercept(IInvocation invocation)
        {
            using (var transaction = _transactionContext.Open())
            {
                invocation.Proceed();
                transaction.Complete();
            }
        }
    }
}
