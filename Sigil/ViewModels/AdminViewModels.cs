using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.ViewModels
{
    public struct OrgAdminIndexViewModel
    {
        public Org thisOrg { get; set; }
    }

    public struct SigilAdminIndexViewModel
    {
        public IEnumerable<Tuple<Org, int>> AllOrgsWithProduct { get; set; }
    }

}