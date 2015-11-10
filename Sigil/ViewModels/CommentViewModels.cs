using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int votes { get; set; }
        public bool userVoted { get; set; }
        public string CommentUserDisplayName { get; set; }
        public string CommentUserIcon { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
    }
}