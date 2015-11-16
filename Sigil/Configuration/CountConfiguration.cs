using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class CommentCountConfiguration : EntityTypeConfiguration<CommentCount>
    {
        public CommentCountConfiguration()
        {
            ToTable("CommentCounts");
            Property(c => c.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.OrgId).IsRequired();
            Property(c => c.IssueId).IsRequired();
            Property(c => c.count).IsRequired();
        }
    }

    public class SubscriptionCountConfiguration : EntityTypeConfiguration<SubCount>
    {
        public SubscriptionCountConfiguration()
        {
            ToTable("SubCounts");
            Property(c => c.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.OrgId).IsRequired();
            Property(c => c.IssueId).IsRequired();
            Property(c => c.count).IsRequired();
        }
    }

    public class ViewCountConfiguration : EntityTypeConfiguration<ViewCount>
    {
        public ViewCountConfiguration()
        {
            ToTable("ViewCounts");
            Property(c => c.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.OrgId).IsRequired();
            Property(c => c.IssueId).IsRequired();
            Property(c => c.count).IsRequired();
        }
    }

    public class VoteCountConfiguration : EntityTypeConfiguration<VoteCount>
    {
        public VoteCountConfiguration()
        {
            ToTable("VoteCounts");
            Property(c => c.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.OrgId).IsRequired();
            Property(c => c.IssueId).IsRequired();
            Property(c => c.count).IsRequired();
        }
    }
}