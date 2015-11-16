using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class NotificationConfiguration : EntityTypeConfiguration<Notification>
    {
        public NotificationConfiguration()
        {
            ToTable("Notifications");
            Property(n => n.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(n => n.From_UserId).IsRequired();
            Property(n => n.To_UserId).IsOptional();
            Property(n => n.To_OrgId).IsOptional();
            Property(n => n.createTime).IsRequired();
            Property(n => n.issueId).IsRequired();
            Property(n => n.orgId).IsRequired();
            Property(n => n.CommentId).IsRequired();
            Property(n => n.NoteType).IsRequired();
        }
    }
}