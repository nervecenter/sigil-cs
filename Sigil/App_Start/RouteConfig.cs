using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sigil {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
             name: "Org",
             url: "{orghandle}/",
             defaults: new { controller = "Org", action = "OrgPage" }
            );

            routes.MapRoute(
                name: "VoteUp",
                url: "voteup/{issueID}/{userhandle}",
                defaults: new { controller = "Issue", action = "VoteUp" }
            );

            routes.MapRoute(
                name: "UnVoteUp",
                url: "unvoteup/{issueID}/{userhandle}",
                defaults: new { controller = "Issue", action = "UnVoteUp" }
            );

            routes.MapRoute(
                name: "AddIssue",
                url: "{orghandle}/addissue",
                defaults: new { controller = "Issue", action = "AddIssue" }
            );

            routes.MapRoute(
                name: "OrgData",
                url: "{orghandle}/data",
                defaults: new { controller = "Org", action = "OrgData" }
            );

            routes.MapRoute(
                name: "Issue",
                url: "{orghandle}/{issueID}",
                defaults: new { controller = "Issue", action = "IssuePage" }
            );

            routes.MapRoute(
                name: "IssueData",
                url: "{orghandle}/{issueID}/data",
                defaults: new { controller = "Issue", action = "IssueData" }
            );

        }
    }
}
