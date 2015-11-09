using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Topic
    {
        public int Id { get; set; }

        public string topicName { get; set; }
        public string topicURL { get; set; }
        public DateTime lastAdded { get; set; }
        public int views { get; set; }

        public virtual List<Org> Orgs { get; set; }
        public virtual List<Category> Categories { get; set; }

    }
}