using System;
using System.Collections.Generic;
using System.Linq;
using Sigil.Models;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace Sigil.ViewModels
{
    public class Home_IndexViewModel
    {
        public UserViewModel UserVM { get; set; }
        public IPagedList<IssuePanelPartialVM> UserIssues { get; set; }
    }

    public class Home_LandingPageViewModel
    {
        //public List<IGrouping<Org, IssuePanelPartialVM>> TrendingIssues { get; set; }
        public List<IGrouping<Org, IssuePanelPartialVM>> LeftColumn { get; set; }
        public List<IGrouping<Org, IssuePanelPartialVM>> MiddleColumn { get; set; }
        public List<IGrouping<Org, IssuePanelPartialVM>> RightColumn { get; set; }

        public Home_LandingPageViewModel()
        {
            LeftColumn = new List<IGrouping<Org, IssuePanelPartialVM>>();
            MiddleColumn = new List<IGrouping<Org, IssuePanelPartialVM>>();
            RightColumn = new List<IGrouping<Org, IssuePanelPartialVM>>();
        }
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
    }
    public class Home_FeaturePageViewModel
    {
        public UserViewModel UserVm
        {
            get; set;
        }
    }
    public class Home_LegalPageViewModel
    {
        public UserViewModel UserVm
        {
            get; set;
        }
    }
}
