using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Sigil.Configuration
{
    public class ErrorConfiguration : EntityTypeConfiguration<Error>
    {
        public ErrorConfiguration()
        {
            ToTable("Errors");
            Property(e => e.Id).IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(e => e.error_object).IsRequired();
            Property(e => e.error_date).IsRequired();
            Property(e => e.error_exception).IsRequired();
        }
    }
}