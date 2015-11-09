using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class TopicConfiguration : EntityTypeConfiguration<Topic>
    {
        public TopicConfiguration()
        {
            ToTable("Topics");
            Property(t => t.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(t => t.topicURL).IsRequired();
            Property(t => t.topicName).IsRequired();
            Property(t => t.lastAdded).IsOptional();
            Property(t => t.views).IsRequired();
        }
    }
}