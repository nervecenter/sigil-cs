using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using PagedList;

namespace Sigil.ViewModels
{
    
    public struct ProductPageViewModel
    {
        public Org thisOrg { get; set; }
        public Product thisProduct { get; set; }
        public UserViewModel UserVM { get; set; }

        public IPagedList<IssuePanelPartialVM> productIssues { get; set; }

    }
}