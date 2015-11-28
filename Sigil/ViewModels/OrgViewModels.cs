using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public struct OrgPageViewModel
    {
        //UserViewModel CurrentUser { get; set; }

        public Org thisOrg { get; set; }

        ICollection<IssuePanelPartialVM> OrgIssues { get; set; }
    }

    public struct OrgDataPageViewModel
    {

    }

    public struct OrgResponsePageViewModel
    {

    }

}