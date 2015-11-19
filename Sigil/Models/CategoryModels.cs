using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string catName { get; set; }
        public string catURL { get; set; }

        public int OrgId { get; set; }
        [ForeignKey("OrgId")]
        public Org Org { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }

        public virtual List<Issue> Issues { get; set; }
    }
}