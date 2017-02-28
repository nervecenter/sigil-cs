using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        public string topicName { get; set; }
        public string topicURL { get; set; }
        public DateTime lastAdded { get; set; }
        public int views { get; set; }

        public int? ImageId { get; set; }
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }

        //public virtual List<Org> Orgs { get; set; }
        //public virtual List<Product> Categories { get; set; }

    }
}