using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public DateTime createTime { get; set; }
        public DateTime editTime { get; set; }

        public string text { get; set; }

        public int IssueId { get; set; }
        [ForeignKey("IssueId")]
        public virtual Issue Issue { get; set; }

        public int votes { get; set; }
        public DateTime lastVoted { get; set; }
    }


    public class OfficialResponse
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        //public int OrgId { get; set; }
        //public Org Org { get; set; }

        public int IssueId { get; set; }
        [ForeignKey("IssueId")]
        public virtual Issue Issue { get; set; }

        public DateTime createTime { get; set; }
        public DateTime editTime { get; set; }

        public string text { get; set; }

        public int upVotes { get; set; }
        public int downVotes { get; set; }

        public DateTime lastVoted { get; set; }
    }

}