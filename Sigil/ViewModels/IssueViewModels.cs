using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public class Issue_IssuePageViewModel
    {


        public IssueViewModel IssueVM { get; set; }

        public UserViewModel UserVM { get; set; }

        public IEnumerable<CommentViewModel> IssueComments { get; set; }
        public IEnumerable<OfficialResponseViewModel> OfficialResponses { get; set; }
    }


    public class Issue_DataPageViewModel
    {

    }



}