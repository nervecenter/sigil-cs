using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class SubscriptionConfiguration : EntityTypeConfiguration<Subscription>
    {
        public SubscriptionConfiguration()
        {
            ToTable("Subscriptions");
            Property(s => s.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(s => s.OrgId).IsRequired();
            Property(s => s.TopicId).IsRequired();
            Property(s => s.CatId).IsRequired();
            Property(s => s.UserId).IsRequired();
        }
    }
}