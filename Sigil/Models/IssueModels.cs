using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set;}

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

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

        public Issue()
        {
            Comments = new List<Comment>();
            OfficialResponses = new List<OfficialResponse>();
        }
    }
}