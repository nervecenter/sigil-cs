using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int OrgId { get; set; }
        public Org Org { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        public int CatId { get; set; }  
        public Category Category { get; set; }

    }
}