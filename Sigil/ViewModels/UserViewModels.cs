using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public class UserHomeViewModel
    {
        ApplicationUser HomeUser { get; set; }

        ICollection<string> Subscriptions { get; set; }
        ICollection<IssueViewModel> UserIssues { get; set; }
    }

    public class LandingPageViewModel
    {
        ICollection<OrgViewModel> TrendingOrgsandIssues { get; set; }
    }

}