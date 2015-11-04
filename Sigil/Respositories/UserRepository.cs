using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    public class UserRepository : RepositoryBase<AspNetUser>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public string GetDisplayName(string userId)
        {
            string displayName = this.DbContext.Users.SingleOrDefault(u => u.Id == userId).DisplayName;
            return displayName;
        }
        //where we define the User methods created below
    }

    public interface IUserRepository : IRepository<AspNetUser>
    {
        //Methods for how when we need to get Users

        string GetDisplayName(string userId);
        
    }
}