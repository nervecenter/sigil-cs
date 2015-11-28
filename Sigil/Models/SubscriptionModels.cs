using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int? OrgId { get; set; }
        [ForeignKey("OrgId")]
        public virtual Org Org { get; set; }

        public int? TopicId { get; set; }
        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; }

        public int? ProductId { get; set; }  
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }
}