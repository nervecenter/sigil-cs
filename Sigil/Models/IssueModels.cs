using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set;}

        public int OrgId { get; set; }
        public Org Org { get; set; }

        public int CatId { get; set; }
        public Category Category { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime createTime { get; set; }
        public DateTime editTime { get; set; }

        public string title { get; set; }
        public string text { get; set; }

        public bool responded { get; set; }

        public int votes { get; set; }
        public DateTime lastVoted { get; set; }
        public int viewCount { get; set; }

        public virtual List<Comment> Comments { get; set; }
        public virtual List<OfficialResponse> OfficialResponses { get; set; }
    }
}