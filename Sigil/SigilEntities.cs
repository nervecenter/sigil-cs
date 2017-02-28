using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Configuration;
using System.Xml.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Sigil
{
    public class SigilEntities : IdentityDbContext<ApplicationUser>//IdentityDbContext<ApplicationUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public SigilEntities() : base("SigilDB", throwIfV1Schema: false)
        {
            Database.SetInitializer<SigilEntities>(new DropCreateDatabaseIfModelChanges<SigilEntities>());//
             // (new CreateDatabaseIfNotExists<SigilEntities>());//new //*/// 

            //    Configuration.ProxyCreationEnabled = false;
            //    Configuration.LazyLoadingEnabled = false;
        }

        public static SigilEntities Create()
        {
            return new SigilEntities();
        }


        public DbSet<Org> Orgs { get; set; }
        public DbSet<Issue> Issues { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Topic> Topics { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<OfficialResponse> OfficialResponses { get; set; }

        public DbSet<ViewCount> ViewCountData { get; set; }
        public DbSet<VoteCount> VoteCountData { get; set; }
        public DbSet<SubCount> SubscriptionCountData { get; set; }
        public DbSet<CommentCount> CommentCountData { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Error> Errors { get; set; }
        public DbSet<OrgApp> OrgApplicants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Configurations.Add(new ProductConfiguration());
            modelBuilder.Configurations.Add(new OrgConfiguration());
            modelBuilder.Configurations.Add(new IssueConfiguration());
            modelBuilder.Configurations.Add(new TopicConfiguration());
            modelBuilder.Configurations.Add(new ViewCountConfiguration());
            modelBuilder.Configurations.Add(new VoteCountConfiguration());
            modelBuilder.Configurations.Add(new SubscriptionCountConfiguration());
            modelBuilder.Configurations.Add(new CommentCountConfiguration());
            modelBuilder.Configurations.Add(new CommentConfiguration());
            modelBuilder.Configurations.Add(new OrgAppConfiguration());
            modelBuilder.Configurations.Add(new ErrorConfiguration());
            modelBuilder.Configurations.Add(new ImageConfiguration());
            modelBuilder.Configurations.Add(new NotificationConfiguration());
            modelBuilder.Configurations.Add(new SubscriptionConfiguration());
            modelBuilder.Configurations.Add(new OfficialResponseConfiguration());

            modelBuilder.Entity<ApplicationUser>().HasOptional(u => u.Image).WithMany().HasForeignKey(u => u.ImageId).WillCascadeOnDelete(false);

        }

        public virtual void Commit()
        {
            try {
                base.SaveChanges();
            }
            catch (Exception e)
            {
                string exp = e.Message;
            }
        }
    }
}