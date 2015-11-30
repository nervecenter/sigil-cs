﻿using System;
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
                name: "Features",
                url: "features/",
                defaults: new { controller = "Home", action = "FeaturesPage" }
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

            routes.MapRoute(
                name: "User Icon Upload",
                url: "user_icon_upload/",
                defaults: new { controller = "ImageUploader", action = "User_Icon_Upload", }
            );

            routes.MapRoute(
                name: "Check Notifications",
                url: "check_notes/",
                defaults: new { controller = "Notification", action = "Get_Notifications", }
            );

            routes.MapRoute(
                name: "Create Issue",
                url: "createissue/",
                defaults: new { controller = "Issue", action = "CreateIssue" }
            );

            routes.MapRoute(
                name: "Add Issue",
                url: "addissue/",
                defaults: new { controller = "Issue", action = "AddIssue" }
            );

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
                name: "Issue Search",
                url: "issueSearch/{orgid}",
                defaults: new { controller = "Search", action = "SearchIssuesByOrg" }
            );

            routes.MapRoute(
                name: "Subscriptions",
                url: "subscriptions/",
                defaults: new { controller = "Subscriptions", action = "Index" }
            );

            routes.MapRoute(
                name: "Search Orgs",
                url: "search/Orgs/",
                defaults: new { controller = "Search", action = "SearchOrgs" }
            );

            routes.MapRoute(
                name: "Search Orgs and Products",
                url: "search/Orgs_Cats/",
                defaults: new { controller = "Search", action = "SearchOrgs_Cats" }
            );

            routes.MapRoute(
                name: "Search Page",
                url: "search/all/",
                defaults: new { controller = "Search", action = "Index" }
            );

            routes.MapRoute(
                 name: "Org Applications",
                 url: "sadmin/orgapps",
                 defaults: new { controller = "Admin", action = "OrgApplicants" }
             );

            routes.MapRoute(
                name: "Org App Approve",
                url: "sadmin/orgapps/{norgID}",
                defaults: new { controller = "Account", action = "OrgConfirmed" }
            );

            routes.MapRoute(
                name: "Error Log",
                url: "sadmin/errors",
                defaults: new { controller = "Admin", action = "ErrorLog" }
            );


            //========================= Subscription Controller ==================================================================
       
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

            //========================= Org Controller ==================================================================

            routes.MapRoute(
                name: "Default Org Data",
                url: "default_graph/{orgURL}/",
                defaults: new { controller = "Org", action = "DefaultData" }
            );

            routes.MapRoute(
                name: "VoteUp",
                url: "voteup/{orgId}/{productId}/{IssueId}/",
                defaults: new { controller = "Issue", action = "VoteUp" }
            );

            routes.MapRoute(
                name: "UnVoteUp",
                url: "unvoteup/{orgId}/{productId}/{IssueId}/",
                defaults: new { controller = "Issue", action = "UnVoteUp" }
            );

            //routes.MapRoute(
            //    name: "Default Issue Data",
            //    url: "default_graph/{orgURL}/{issueId}",
            //    defaults: new { controller = "Issue", action = "DefaultData" }
            //);

            routes.MapRoute(
                name: "Custom Org Data",
                url: "custom_graph/{orgURL}/{dataType}/{start}/{stop}/",
                defaults: new { controller = "Org", Action = "CustomData" }
            );

            routes.MapRoute(
                name: "Custom Issue Data",
                url: "custom_graph/{orgURL}/{issueId}/{dataType}/{start}/{stop}/",
                defaults: new { controller = "Issue", Action = "CustomData" }
            );

            routes.MapRoute(
                name: "OrgData",
                url: "{orgURL}/data/",
                defaults: new { controller = "Org", action = "OrgData" }
            );

            routes.MapRoute(
                name: "OrgResponses",
                url: "{orgURL}/responses/",
                defaults: new { controller = "Org", action = "OrgResponsesPage" }
            );

            routes.MapRoute(
              name: "Org",
              url: "{orgURL}/",
              defaults: new { controller = "Org", action = "OrgPage" }
            );

            //routes.MapRoute(
            //    name: "Org Product Page",
            //    url: "{orgURL}/{catURL}/",
            //    defaults: new { controller = "Org", action = "CatPage" }
            //);

            //========================= Issue Controller ==================================================================


            routes.MapRoute(
                name: "Issue",
                url: "{orgURL}/{productURL}/{IssueId}/",
                defaults: new { controller = "Issue", action = "IssuePage" }
            );

            //routes.MapRoute(
            //    name: "IssueData",
            //    url: "{orgURL}/{pro{IssueId}/data/",
            //    defaults: new { controller = "Issue", action = "IssueData" }
            //);

       

            //========================= Default Controller ==================================================================

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
