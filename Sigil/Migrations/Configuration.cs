namespace Sigil.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Sigil.SigilEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Sigil.SigilEntities";
        }

        protected override void Seed(Sigil.SigilEntities context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            var roles = CreateRoles();

            foreach( var role in roles)
            {
                if(!(context.Roles.Any(r => r.Name == role.Name)))
                {
                    context.Roles.Add(role);
                }
            }

            if(!(context.Orgs.Any(o => o.orgName == "Sigil")))
            {
                var sigilOrg = new Org();
                sigilOrg.orgName = "Sigil";
                sigilOrg.orgURL = "sigil";
                sigilOrg.viewCount = 0;
                sigilOrg.lastView = DateTime.UtcNow;
                context.Orgs.AddOrUpdate(sigilOrg);
                context.SaveChanges();
            }

            if (!(context.Users.Any(u => u.UserName == "dominic@sigil.tech")))
            {

                
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser { UserName = "dominic@sigil.tech", Email = "dominic@sigil.tech", DisplayName = "Dominic" };
                userManager.Create(userToInsert, "s323232");
                var user = context.Users.Where(u => u.UserName == "dominic@sigil.tech").FirstOrDefault();
                user.OrgId = context.Orgs.Where(o => o.orgName == "Sigil").FirstOrDefault().Id;
                Image userImg = new Image();
                userImg.imgType = (int)ImageTypeOwner.User;
                userImg.OwnerId = user.Id;
                context.Images.AddOrUpdate(userImg);
                context.SaveChanges();
                userImg = context.Images.Where(i => i.OwnerId == user.Id).FirstOrDefault();
                user.ImageId = userImg.Id;
                user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol()).ToString();
                context.Users.AddOrUpdate(user);
                userManager.AddToRole(user.Id, "SigilAdmin");
            }

            if (!(context.Users.Any(u => u.UserName == "nervecenter7@gmail.com")))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser { UserName = "cjcollazo@sigil.tech", Email = "cjcollazo@sigil.tech", DisplayName = "Chris" };
                userManager.Create(userToInsert, "s323232");

                var user = context.Users.Where(u => u.UserName == "nervecenter7@gmail.com").FirstOrDefault();
                user.OrgId = context.Orgs.Where(o => o.orgName == "Sigil").FirstOrDefault().Id;
                Image userImg = new Image();
                userImg.imgType = (int)ImageTypeOwner.User;
                userImg.OwnerId = user.Id;
                context.Images.AddOrUpdate(userImg);
                context.SaveChanges();
                userImg = context.Images.Where(i => i.OwnerId == user.Id).FirstOrDefault();
                user.ImageId = userImg.Id;
                user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol()).ToString();
                context.Users.AddOrUpdate(user);
                userManager.AddToRole(user.Id, "SigilAdmin");

            }
            if(!(context.Users.Any(u => u.UserName == "deleted@sigil.tech")))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var userToInsert = new ApplicationUser { UserName = "deleted@sigil.tech", Email = "deleted@sigil.tech", DisplayName = "Deleted" };
                userManager.Create(userToInsert, "s323232");

                var user = context.Users.Where(u => u.UserName == "deleted@sigil.tech").FirstOrDefault();

                Image userImg = new Image();
                userImg.imgType = (int)ImageTypeOwner.User;
                userImg.OwnerId = user.Id;
                context.Images.AddOrUpdate(userImg);
                context.SaveChanges();
                userImg = context.Images.Where(i => i.OwnerId == user.Id).FirstOrDefault();
                user.ImageId = userImg.Id;
                user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol()).ToString();
                context.Users.AddOrUpdate(user);
                userManager.AddToRole(user.Id, "SigilAdmin");
            }

            context.SaveChanges();

        }

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

      

    }
}
