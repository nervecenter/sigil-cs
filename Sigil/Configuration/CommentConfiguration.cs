using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class CommentConfiguration : EntityTypeConfiguration<Comment>
    {
        public CommentConfiguration()
        {
            ToTable("Comments");
            Property(c => c.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.UserId).IsRequired();
            Property(c => c.createTime).IsRequired();
            Property(c => c.editTime).IsOptional();
            Property(c => c.text).IsRequired();
            Property(c => c.IssueId).IsRequired();
            Property(c => c.votes).IsRequired();
            Property(c => c.lastVoted).IsRequired();
            
        }
    }
    
    
    public class OfficialResponseConfiguration : EntityTypeConfiguration<OfficialResponse>
    {
        public OfficialResponseConfiguration()
        {
            ToTable("OfficialResponse");
            Property(o => o.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(o => o.upVotes).IsRequired();
            Property(o => o.downVotes).IsRequired();
            Property(o => o.UserId).IsRequired();
            Property(o => o.createTime).IsRequired();
            Property(o => o.text).IsRequired();
            Property(o => o.editTime).IsOptional();
            Property(o => o.issueId).IsRequired();
        }
    }

}