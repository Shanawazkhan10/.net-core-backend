using TransactionMicroservice.Model;
using TransactionMicroservice.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace TransactionMicroservice.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private TransactionRepositoryContext _repoContext;
        public TransactionRepository(TransactionRepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }

}
