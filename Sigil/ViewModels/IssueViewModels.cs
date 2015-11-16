using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public class IssuePageViewModel
    {
        public int IssueId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int votes { get; set; }
        public bool userVoted { get; set; }
        public bool Responded { get; set; }
        public string IssueUserDisplayName { get; set; }
        public string IssueUserIcon { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }

        ICollection<CommentViewModel> IssueComments { get; set; }
    }


    public class IssueViewModel
    {
        public int IssueId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int votes { get; set; }
        public bool userVoted { get; set; }
        public bool Responded { get; set; }
        public string IssueUserDisplayName { get; set; }
        public string IssueUserIcon { get; set; }
        public string Title { get; set; }
        //public string Text { get; set; }
        public DateTime CreateDate { get; set; }
    }


}