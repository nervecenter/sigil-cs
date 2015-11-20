using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace Sigil.ViewModels
{
    public class Home_IndexViewModel
    {
        public UserViewModel UserVM { get; set; }
        public IPagedList<IssueViewModel> UserIssues { get; set; }
    }

    public class Home_LandingPageViewModel
    {
        public IEnumerable<IGrouping<OrgViewModel, IssueViewModel>> TrendingIssues { get; set; }
    }

    public class Home_AboutPageViewModel
    {
        public UserViewModel UserVm { get; set; }
    }

    public class Home_ContactPageViewModel
    {
        public UserViewModel UserVm
        {
            get; set;
        }

    public class Home_FeaturePageViewModel
    {
        public UserViewModel UserVm
        {
            get; set;
        }

    public class Home_LegalPageViewModel
    {
        public UserViewModel UserVm
        {
            get; set;
        }
}
