using HelperMicroservice.Model;
using HelperMicroservice.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HelperMicroservice.Repository
{
    public class HelperRepository : IHelperRepository
    {
        private HelperRepositoryContext _repoContext;
        public HelperRepository(HelperRepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }
}
