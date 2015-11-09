using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string From_UserId { get; set; }
        
        public string To_UserId { get; set; }
        public int To_OrgId { get; set; }
        public DateTime createTime { get; set; }

        public int issueId { get; set; }
        public Issue Issue { get; set; }

        public int orgId { get; set; }
        public Org Org { get; set; }

        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public int NoteType { get; set; }
    }
}