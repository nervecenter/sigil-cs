using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public class OrgViewModel
    {
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgURL { get; set; }
        public string OrgIcon { get; set; }

        ICollection<IssueViewModel> OrgIssues { get; set; }
    }
}