using MasterDataMicroservice.Model;
using MasterDataMicroservice.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MasterDataMicroservice.Repository
{
    public class MasterRepository : IMasterRepository
    {
        private MasterRepositoryContext _repoContext;
        public MasterRepository(MasterRepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }

}
