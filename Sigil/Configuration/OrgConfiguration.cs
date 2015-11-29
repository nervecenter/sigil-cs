using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class OrgConfiguration : EntityTypeConfiguration<Org>
    {
        public OrgConfiguration()
        {
            ToTable("Orgs");
            Property(o => o.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(o => o.orgName).IsRequired();
            Property(o => o.orgURL).IsRequired().HasMaxLength(30);
            Property(o => o.ImageId).IsOptional();
            Property(o => o.viewCount).IsRequired();
            Property(o => o.website).IsOptional();
            Property(o => o.lastView).IsRequired();
            Property(o => o.UserID).IsOptional();
            
            
            
            
        }
    }

    public class OrgAppConfiguration : EntityTypeConfiguration<OrgApp>
    {
        public OrgAppConfiguration()
        {
            ToTable("OrgApps");
            Property(o => o.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(o => o.orgName).IsRequired();
            Property(o => o.orgURL).IsRequired().HasMaxLength(30);
            Property(o => o.website).IsOptional();
            Property(o => o.comment).IsOptional();

            Property(o => o.DisplayName).IsRequired();
            Property(o => o.AdminEmail).IsRequired();
            Property(o => o.ContactNumber).IsRequired();
            Property(o => o.AdminContactName).IsRequired();
            Property(o => o.ApplyDate).IsRequired();
            Property(o => o.OrgApproved).IsRequired();
            
            
        }
    }
}