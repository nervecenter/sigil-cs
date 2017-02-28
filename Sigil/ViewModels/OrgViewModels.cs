using Sigil.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Sigil.ViewModels
{
    public struct OrgPageViewModel
    {
        //UserViewModel CurrentUser { get; set; }

        public Org thisOrg { get; set; }
        public UserViewModel UserVM { get; set; }
        public IPagedList<IssuePanelPartialVM> orgIssues { get; set; }
    }

    public struct OrgResponsesViewModel {
        //UserViewModel CurrentUser { get; set; }

        public Org thisOrg { get; set; }
        public UserViewModel UserVM { get; set; }
        public IPagedList<IssuePanelPartialVM> orgRespondedIssues { get; set; }
    }

    public struct OrgListViewModel {
        public IEnumerable<Org> orgs { get; set; }
        public UserViewModel UserVM { get; set; }
    }

    public struct OrgDataPageViewModel
    {
        public Org thisOrg { get; set; }
        public IEnumerable<IssuePanelPartialVM> TopIssuePeriod { get; set; }
        public IEnumerable<IssuePanelPartialVM> TopNewRising { get; set; }
        public IEnumerable<IssuePanelPartialVM> TopWaitingResponse { get; set; }
    }

}