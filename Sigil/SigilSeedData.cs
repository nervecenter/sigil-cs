using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sigil
{
    public class SigilSeedData : DropCreateDatabaseIfModelChanges<SigilEntities>
    {
        protected override void Seed(SigilEntities context)
        {
            GetOrgs().ForEach(o => context.Orgs.Add(o));
            GetCats().ForEach(c => context.Categories.Add(c));
            CreateRoles().ForEach(r => context.Roles.Add(r));
            //base.Seed(context);


            context.Commit();
            context.SaveChanges();

            
        }

        //This is where we can add initial data 

        private static List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> CreateRoles()
        {
            return new List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>
            {
                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = "SigilAdmin"
                },
                
                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = "OrgSuperAdmin"
                },

                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = "OrgAdmin"
                },

                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = "BasicUser"
                }
            };
        }


        private static List<Org> GetOrgs()
        {
            return new List<Org>
            {
                new Org
                {
                    Products = GetCats(),
                    orgName = "Micro",
                    orgURL = "mic",
                    lastView = DateTime.UtcNow,
                    viewCount = 1,
                    //Topicid = 0,
                },
                new Org
                {
                    Products = GetCats(),
                    orgName = "Star",
                    orgURL = "star",
                    lastView = DateTime.UtcNow,
                    viewCount = 1,
                    //Topicid = 0,
                }
            };
        }

        private static List<Product> GetCats()
        {
            return new List<Product>
            {
                new Product
                {
                    ProductName = "Coffee",
                    ProductURL = "coffee",
                },
                new Product
                {
                    ProductName = "Software",
                    ProductURL = "soft",

                }
            };
        }
    }
}