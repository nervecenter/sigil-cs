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
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
                );

            // Only redirected to LandingPage if not logged in
            //routes.MapRoute(
            //    name: "Landing",
            //    url: "",
            //    defaults: new { controller = "Home", action = "LandingPage" }
            //    );

            routes.MapRoute(
                name:"Login",
                url: "login/",
                defaults: new { controller = "Account", action="Login"}
                );

            routes.MapRoute(
                name: "Logout",
                url: "logout/",
                defaults: new { controller = "Account", action = "LogOff" }
                );

            routes.MapRoute(
                name: "Register",
                url: "register/",
                defaults: new { controller = "Account", action = "Register" }
                );

            routes.MapRoute(
                name: "Legal",
                url: "legal/",
                defaults: new { controller = "Home", action = "Legal" }
                );

            routes.MapRoute(
                name: "Search for Dropdown",
                url: "search/",
                defaults: new { controller = "Search", action = "SearchDB" }
                );

            routes.MapRoute(
                name: "Search Page",
                url: "search/{term}",
                defaults: new { controller = "Search", action = "Index" }
                );

            routes.MapRoute(
                name: "Search Orgs",
                url: "search/",
                defaults: new { controller = "Search", action = "SearchOrgs" }
                );

            routes.MapRoute(
                name: "VoteUp",
                url: "voteup/{IssueId}/",
                defaults: new { controller = "Issue", action = "VoteUp" }
            );

            routes.MapRoute(
                name: "UnVoteUp",
                url: "unvoteup/{IssueId}/",
                defaults: new { controller = "Issue", action = "UnVoteUp" }
            );
            routes.MapRoute(
               name: "Subscriptions",
               url: "subscriptions/",
               defaults: new { controller = "Subscriptions", action = "Index" }
           );

            routes.MapRoute(
                name: "Subscribe",
                url: "{orgURL}/subscribe/",
                defaults: new { controller = "Subscriptions", action = "AddSubscription" }
            );

            routes.MapRoute(
                name: "UnSubscribe",
                url: "{orgURL}/unsubscribe/",
                defaults: new { controller = "Subscriptions", action = "DeleteSubscription" }
            );

            routes.MapRoute(
                name: "AddIssue",
                url: "create_issue/",
                defaults: new { controller = "Issue", action = "AddIssue" }
            );

            routes.MapRoute(
                name: "OrgData",
                url: "{orgURL}/data/",
                defaults: new { controller = "Org", action = "OrgData" }
            );

            routes.MapRoute(
                name: "Issue",
                url: "{orgURL}/{IssueId}/",
                defaults: new { controller = "Issue", action = "IssuePage" }
            );

            routes.MapRoute(
                name: "IssueData",
                url: "{orgURL}/{IssueId}/data/",
                defaults: new { controller = "Issue", action = "IssueData" }
            );

            routes.MapRoute(
              name: "Org",
              url: "{orgURL}/",
              defaults: new { controller = "Org", action = "OrgPage" }
             );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
