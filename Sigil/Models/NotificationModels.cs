using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string From_UserId { get; set; }
        
        public string To_UserId { get; set; }
        public int To_OrgId { get; set; }

        public DateTime createTime { get; set; }

        public int issueId { get; set; }

        public int productId { get; set; }

        public int orgId { get; set; }

        public int CommentId { get; set; }

        public int NoteType { get; set; }
    }
}