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
                name: "Features",
                url: "features/",
                defaults: new { controller = "Home", action = "FeaturesPage" }
            );

            routes.MapRoute(
                name: "404",
                url: "404/",
                defaults: new { controller = "Shared", action = "_404" }
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
                defaults: new { controller = "Manage", action = "User_Icon_Upload", }
            );

            routes.MapRoute(
                name: "Check Notifications",
                url: "check_notes/",
                defaults: new { controller = "Notification", action = "Get_Notifications", }
            );

            routes.MapRoute(
                name: "Add Issue",
                url: "addissue/",
                defaults: new { controller = "Issue", action = "AddIssue_Post" }
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
               name: "Subscriptions",
               url: "subscriptions/",
               defaults: new { controller = "Subscriptions", action = "Index" }
           );

            routes.MapRoute(
                name: "Manage",
                url: "manage/",
                defaults: new { controller = "Manage", action = "Index" }
            );

            routes.MapRoute(
                name: "Change Password",
                url: "manage/changepassword/",
                defaults: new { controller = "Manage", action = "ChangePassword" }
            );

            //========================= Search Controller ==================================================================
            routes.MapRoute(
                name: "Search for Header Dropdown",
                url: "search/",
                defaults: new { controller = "Search", action = "SearchDB" }
            );

            routes.MapRoute(
               name: "Search Issues by Org",
               url: "searchissuesbyorg/",
               defaults: new { controller = "Search", action = "SearchIssuesByOrg" }
            );

            routes.MapRoute(
               name: "Search Issues by Product",
               url: "searchissuesbyproduct/",
               defaults: new { controller = "Search", action = "SearchIssuesByProduct" }
            );

            routes.MapRoute(
                name: "Admin Index",
                url: "admin/",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapRoute(
                name: "Sigil Admin Index",
                url: "sadmin/",
                defaults: new { controller = "Admin", action = "SigilAdminIndex" }
            );

            routes.MapRoute(
                name: "User DisplayName Search",
                url: "search/AdminUserSearch",
                defaults: new { controller = "Search", action = "SearchByUserDisplayName" }
            );

            routes.MapRoute(
                name: "Admin Product Search",
                url: "search/AdminProductSearch/",
                defaults: new { controller = "Search", action = "AdminProductSearch" }
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
                name: "Error Log",
                url: "sadmin/errors",
                defaults: new { controller = "Admin", action = "ErrorLog" }
            );

            routes.MapRoute(
                name: "Topic Admin Area",
                url: "sadmin/topicAdmin",
                defaults: new { controller = "Admin", action = "TopicAdmin" }
            );

            routes.MapRoute(
                name: "Create Topic",
                url: "sadmin/topicAdmin/createTopic",
                defaults: new { controller = "Admin", action = "CreateTopic" }
            );

            routes.MapRoute(
                name: "Assign Product To Topic",
                url: "sadmin/topicAdmin/AssignProduct",
                defaults: new { controller = "Admin", action = "AssignProductToTopic" }
            );

            routes.MapRoute(
               name: "Admin View Roles",
               url: "sadmin/roles/",
               defaults: new { controller = "Admin", action = "RolesIndex" }
           );

            routes.MapRoute(
                name: "Create Role",
                url: "sadmin/roles/create/",
                defaults: new { controller = "Admin", action = "CreateRole" }
            );

            //routes.MapRoute(
            //    name: "Role Manager",
            //    url: "sadmin/roles/manage/",
            //    defaults: new { controller = "Admin", action = "ManageUserRoles" }
            //);

            routes.MapRoute(
                name: "Topic Page",
                url: "t/{topicURL}/",
                defaults: new { controller = "Topic", action = "TopicPage" }
            );



            routes.MapRoute(
                name: "Subscribe",
                url: "subscribe/{URL}/{type}/",
                defaults: new { controller = "Subscriptions", action = "AddSubscription" }
            );

            routes.MapRoute(
                name: "UnSubscribe",
                url: "unsubscribe/{URL}/{type}",
                defaults: new { controller = "Subscriptions", action = "DeleteSubscription" }
            );

            routes.MapRoute(
                name: "Org App Approve",
                url: "sadmin/orgapps/{norgID}",
                defaults: new { controller = "Account", action = "OrgConfirmed" }
            );

           

            routes.MapRoute(
                name: "Default Org Data",
                url: "default_graph/{orgURL}/",
                defaults: new { controller = "Org", action = "DefaultData" }
            );

            routes.MapRoute(
                name: "Issue VoteUp",
                url: "voteup/{IssueId}/",
                defaults: new { controller = "Issue", action = "VoteUp" }
            );

            routes.MapRoute(
                name: "Issue UnVoteUp",
                url: "unvoteup/{IssueId}/",
                defaults: new { controller = "Issue", action = "UnVoteUp" }
            );

            routes.MapRoute(
                name: "Comment VoteUp",
                url: "voteup/{IssueId}/{CommentId}",
                defaults: new { controller = "Issue", action = "VoteUpComment" }
            );

            routes.MapRoute(
                name: "Comment UnVoteUp",
                url: "unvoteup/{IssueId}/{CommentId}/",
                defaults: new { controller = "Issue", action = "UnVoteUpComment" }
            );


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
                name: "OrgAdmin",
                url: "{orgURL}/admin/",
                defaults: new { controller = "Admin", action = "OrgAdmin" }
            );

            routes.MapRoute(
                name: "Org Issue Delete",
                url: "{orgURL}/issuedelete/{issueId}/",
                defaults: new { controller = "Org", action = "DeleteIssue" }
            );

            routes.MapRoute(
                name: "OrgCreateNewProduct",
                url: "{orgURL}/admin/newproduct/",
                defaults: new { controller = "Admin", action = "AddOrgProduct" }
            );

            routes.MapRoute(
                name: "Org 100 Icon Upload",
                url: "{orgURL}/admin/100IconUpload/",
                defaults: new { controller = "Admin", action = "UploadOrgIcon100" }
            );

            routes.MapRoute(
                name: "Org 20 Icon Upload",
                url: "{orgURL}/admin/20IconUpload/",
                defaults: new { controller = "Admin", action = "UploadOrgIcon20" }
            );

            routes.MapRoute(
                name: "Org Banner Upload",
                url: "{orgURL}/admin/bannerUpload/",
                defaults: new { controller = "Admin", action = "UploadOrgBanner" }
            );

            routes.MapRoute(
                name: "Org URL Change",
                url: "{orgURL}/admin/OrgURLChange/",
                defaults: new { controller = "Admin", action = "OrgURLChange" }
            );

            routes.MapRoute(
                name: "Org Admin Product Delete",
                url: "{orgURL}/admin/delete/{productId}/",
                defaults: new { controller = "Admin", action = "DeleteOrgProduct" }
            );

            routes.MapRoute(
              name: "Org Page",
              url: "{orgURL}/",
              defaults: new { controller = "Org", action = "OrgPage" }
            );


            routes.MapRoute(
                name: "Product Page",
                url: "{orgURL}/{productURL}/",
                defaults: new { controller = "Product", action = "ProductPage" }
            );

            routes.MapRoute(
                name: "Product Issue Delete",
                url: "{orgURL}/{productURL}/issueDelete/{issueId}/",
                defaults: new { controller = "Product", action = "DeleteIssue" }
            );

            routes.MapRoute(
                name: "Issue",
                url: "{orgURL}/{productURL}/{IssueId}/",
                defaults: new { controller = "Issue", action = "IssuePage" }
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
