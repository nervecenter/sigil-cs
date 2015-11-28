using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string ProductName { get; set; }
        public string ProductURL { get; set; }

        public int OrgId { get; set; }
        [ForeignKey("OrgId")]
        public Org Org { get; set; }


        public int TopicId { get; set; }
        [ForeignKey("TopicId")]
        public Topic Topic { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }

        public virtual List<Issue> Issues { get; set; }
    }
}