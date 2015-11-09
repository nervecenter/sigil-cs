using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public int orgId { get; set; }
        public Org Org { get; set; }

        public int topicId { get; set; }
        public Topic Topic { get; set; }


    }
}