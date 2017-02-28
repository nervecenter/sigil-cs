using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Sigil.Repository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        //Methods for how when we need to get Users

        string GetDisplayName(string userId);
        ApplicationUser GetById(string id);
        ApplicationUser GetByDisplayName(string name);
        void CreateRole(IdentityRole role);
        IEnumerable<IdentityRole> GetAllRoles();
        IdentityRole GetRole(string roleName);
        void DeleteRole(string roleName);
        void EditRole(string roleName, string newRoleName);

    }


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

        public void CreateRole(IdentityRole role)
        {
            DbContext.Roles.Add(role);
            
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return DbContext.Roles.Select(r => r);
        }

        public IdentityRole GetRole(string roleName)
        {
            return DbContext.Roles.Where(r => r.Name == roleName).FirstOrDefault();
        }

        public void DeleteRole(string roleName)
        {
            var role = GetRole(roleName);
            DbContext.Roles.Remove(role);

        }

        public void EditRole(string roleName, string newRoleName)
        {
            var role = GetRole(roleName);
            role.Name = newRoleName;
            DbContext.Entry(role).State = EntityState.Modified;
        }



        //where we define the User methods created below
    }

    
}