using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {
            ToTable("Product");
            Property(c => c.Id).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(c => c.ProductName).IsRequired().HasMaxLength(128);
            Property(c => c.ProductURL).IsRequired().HasMaxLength(30);
            Property(c => c.OrgId).IsRequired();
            Property(c => c.TopicId).IsOptional();
            Property(c => c.ImageId).IsOptional();
        }
    }
}