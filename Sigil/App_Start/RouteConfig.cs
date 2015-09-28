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


            //========================== Home Controller ==================================================================
            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Only redirected to LandingPage if not logged in
            routes.MapRoute(
                name: "Landing",
                url: "",
                defaults: new { controller = "Home", action = "LandingPage" }
            );

            routes.MapRoute(
                name: "About",
                url: "about/",
                defaults: new { controller = "Home", action = "About" }
            );

            routes.MapRoute(
                name: "Legal",
                url: "legal/",
                defaults: new { controller = "Home", action = "Legal" }
            );

            //========================= Sigil Admin Stuff =================================================

            routes.MapRoute(
                name: "Org Applications",
                url: "sadmin/orgapps",
                defaults: new { controller = "Admin", action = "OrgApplicants" }
            );

            routes.MapRoute(
                name: "Error Log",
                url: "sadmin/errors",
                defaults: new { controller = "Admin", action = "ErrorLog" }
            );

            //========================= Account Controller ==================================================================
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
                name: "OrgRegister",
                url: "orgregister/",
                defaults: new { controller = "Account", action = "OrgRegister" }
            );

            routes.MapRoute(
                name: "Manage",
                url: "manage/",
                defaults: new { controller = "Manage", action = "Index" }
                );
            //========================= Search Controller ==================================================================
            routes.MapRoute(
                name: "Search for Header Dropdown",
                url: "search/",
                defaults: new { controller = "Search", action = "SearchDB" }
            );

            routes.MapRoute(
                name: "Search Orgs",
                url: "search/Orgs/",
                defaults: new { controller = "Search", action = "SearchOrgs" }
            );

            routes.MapRoute(
                name: "Search Orgs and Categories",
                url: "search/Orgs_Cats/",
                defaults: new { controller = "Search", action = "SearchOrgs_Cats" }
            );

            routes.MapRoute(
                name: "Search Page",
                url: "search/all/",
                defaults: new { controller = "Search", action = "Index" }
            );

            //========================= Subscription Controller ==================================================================
            routes.MapRoute(
               name: "Subscriptions",
               url: "subscriptions/",
               defaults: new { controller = "Subscriptions", action = "Index" }
            );
            routes.MapRoute(
                name: "Subscribe",
                url: "subscribe/{orgURL}/",
                defaults: new { controller = "Subscriptions", action = "AddSubscription" }
            );

            routes.MapRoute(
                name: "UnSubscribe",
                url: "unsubscribe/{orgURL}/",
                defaults: new { controller = "Subscriptions", action = "DeleteSubscription" }
            );


            routes.MapRoute(
                name: "Create Issue",
                url: "create_issue/",
                defaults: new { controller = "Issue", action = "CreateIssue" }
            );

            routes.MapRoute(
                name: "Add Issue",
                url: "add_issue/",
                defaults: new { controller = "Issue", action = "AddIssue" }
            );


            //========================= Org Controller ==================================================================
            routes.MapRoute(
                name: "OrgData",
                url: "{orgURL}/data/",
                defaults: new { controller = "Org", action = "OrgData" }
            );

            routes.MapRoute(
              name: "Org",
              url: "{orgURL}/",
              defaults: new { controller = "Org", action = "OrgPage" }
            );

            //routes.MapRoute(
            //    name: "Org Category Page",
            //    url: "{orgURL}/{catURL}/",
            //    defaults: new { controller = "Org", action = "CatPage" }
            //);

            //========================= Issue Controller ==================================================================
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
                name: "Issue",
                url: "{orgURL}/{IssueId}/",
                defaults: new { controller = "Issue", action = "IssuePage" }
            );

            routes.MapRoute(
                name: "IssueData",
                url: "{orgURL}/{IssueId}/data/",
                defaults: new { controller = "Issue", action = "IssueData" }
            );

            //========================= Default Controller ==================================================================

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
