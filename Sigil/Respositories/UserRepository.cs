using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public ApplicationUser GetByDisplayName(string name)
        {
            var user = this.DbContext.Users.Where(u => u.DisplayName == name).FirstOrDefault();
            return user;
        }

        public ApplicationUser GetById(string id)
        {
            var user = this.DbContext.Users.Where(u => u.Id == id).FirstOrDefault();
            return user;
        }

        public string GetDisplayName(string userId)
        {
            string displayName = this.DbContext.Users.SingleOrDefault(u => u.Id == userId).DisplayName;
            return displayName;
        }
        //where we define the User methods created below
    }

    public interface IUserRepository : IRepository<ApplicationUser>
    {
        //Methods for how when we need to get Users

        string GetDisplayName(string userId);
        ApplicationUser GetById(string id);
        ApplicationUser GetByDisplayName(string name);

    }
}