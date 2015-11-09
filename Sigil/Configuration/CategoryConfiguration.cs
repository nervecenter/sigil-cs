using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            ToTable("Category");
            Property(c => c.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.catName).IsRequired().HasMaxLength(128);
            Property(c => c.catURL).IsRequired().HasMaxLength(30);
            Property(c => c.orgId).IsRequired();
            Property(c => c.topicId).IsRequired();
        }
    }
}