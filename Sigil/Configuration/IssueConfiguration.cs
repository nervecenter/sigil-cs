using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class IssueConfiguration : EntityTypeConfiguration<Issue>
    {
        public IssueConfiguration()
        {
            ToTable("Issues");
            Property(i => i.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            //Property(i => i.OrgId).IsOptional();
            Property(i => i.CatId).IsOptional();
            Property(i => i.UserId).IsRequired();
            Property(i => i.createTime).IsRequired();
            Property(i => i.editTime).IsRequired();
            Property(i => i.title).IsRequired().HasMaxLength(128);
            Property(i => i.text).IsRequired();
            Property(i => i.responded).IsRequired();
            Property(i => i.votes).IsRequired();
            Property(i => i.lastVoted).IsRequired();
            Property(i => i.viewCount).IsRequired();
            
            //Property(i => i.TopicId).IsOptional();
            
        }
    }
}