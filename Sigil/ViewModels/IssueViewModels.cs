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
        
        public Org IssueOrg { get; set; }
        
        public Product IssueProduct { get; set; }

        public UserViewModel UserVM { get; set; }

        public SideBarVM sideBar { get; set; }

        public IEnumerable<Tuple<Comment, bool>> IssueComments { get; set; }

        public IEnumerable<OfficialResponse> OfficialResponses { get; set; }
    }


    public struct Issue_DataPageViewModel
    {

    }

    public struct AddIssueVM {
        public Org org;

        public Product product;

        public string title;
        public string text;
        /*public AddIssueVM(Org o, Product p, string title) {
            thisOrg = o;
            thisProduct = p;
            issueTitle = title;
        }*/
    }

}