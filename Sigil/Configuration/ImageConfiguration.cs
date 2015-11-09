using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class ImageConfiguration : EntityTypeConfiguration<Image>
    {
        public ImageConfiguration()
        {
            ToTable("Images");
            Property(i => i.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(i => i.OrgId).IsOptional();
            Property(i => i.UserId).IsOptional();
            Property(i => i.TopicId).IsOptional();
            Property(i => i.CatId).IsOptional();
            Property(i => i.icon_20).IsOptional();
            Property(i => i.icon_100).IsOptional();
            Property(i => i.banner).IsOptional();
        }
    }
}