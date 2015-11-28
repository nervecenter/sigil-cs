using System;
using System.Collections.Generic;
using Sigil.Models;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public struct IssuePageViewModel
    {
        public IssuePanelPartialVM IssuePanelVM { get; set; }
        
        public UserViewModel UserVM { get; set; }

        public SideBarVM sideBar { get; set; }

        public IEnumerable<Tuple<Comment, bool>> IssueComments { get; set; }

        public IEnumerable<OfficialResponse> OfficialResponses { get; set; }
    }


    public struct Issue_DataPageViewModel
    {

    }



}