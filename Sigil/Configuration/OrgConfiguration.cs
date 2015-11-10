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
            Property(o => o.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(o => o.orgName).IsRequired();
            Property(o => o.UserID).IsOptional();
            Property(o => o.viewCount).IsRequired();
            Property(o => o.website).IsOptional();
            Property(o => o.orgURL).IsRequired().HasMaxLength(30);
            Property(o => o.ImageId).IsOptional();
            //Property(o => o.Topicid).IsRequired();


        }
    }

    public class OrgAppConfiguration : EntityTypeConfiguration<OrgApp>
    {
        public OrgAppConfiguration()
        {
            ToTable("OrgApps");
            Property(o => o.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(o => o.orgName).IsRequired();
            Property(o => o.AdminName).IsRequired();
            Property(o => o.username).IsRequired();
            Property(o => o.website).IsOptional();
            Property(o => o.orgURL).IsRequired().HasMaxLength(30);
            Property(o => o.contact).IsRequired();
            Property(o => o.contact).IsOptional();
            
        }
    }
}