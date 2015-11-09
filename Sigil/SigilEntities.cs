using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;
using System.Xml.Linq;

namespace Sigil
{
    public class SigilEntities : DbContext
    {
        public SigilEntities() : base("aspnet-Sigil-20150716011718") { }

        //public DbSet<XElement> xele { get; set; }
        public DbSet<Org> Orgs { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<AspNetUser> Users { get; set; }

        public DbSet<Category> Categories { get; set; }
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

        //public DbSet<AspNetUserRole> UserRoles { get; set; }
        

        public virtual void Commit()
        {
            base.SaveChanges();
        }
    }
}