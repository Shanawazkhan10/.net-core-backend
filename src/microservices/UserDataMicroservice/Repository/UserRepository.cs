using UserDataMicroservice.Model;
using UserDataMicroservice.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserDataMicroservice.Repository
{
    public class UserRepository : IUserRepository
    {
        private User_RepositoryContext _repoContext;
        public UserRepository(User_RepositoryContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
    }

}
