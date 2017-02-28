using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sigil.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string DisplayName { get; set; }

        public int OrgId { get; set; }
        //public Org Org { get; set; }

        public string votes { get; set; }
        //public DateTime lastLogon { get; set; }

        public int? ImageId { get; set; }
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }

        public virtual List<Subscription> Subscriptions { get; set; }

        public ApplicationUser()
        {
            Subscriptions = new List<Subscription>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            
            return userIdentity;
        }
    }

    //The commented out section below has been replaced by SigilEntities.cs file. Keep this here as a reference from where it started.

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }

    //}
}