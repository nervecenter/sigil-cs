using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using PagedList;
using Sigil.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Globalization;
using Sigil.Services;

namespace Sigil.Controllers
{
    public class OrgController : Controller
    {
        private readonly IOrgService orgService;
        private readonly ICountService countDataService;
        private readonly IIssueService issueService;
        private readonly ICommentService commentService;

        /* 
        ==================== 
        OrgPage
  
            Returns a page listing the latest issues specific to an org 
        ==================== 
        */

        public ActionResult OrgPage(string orgURL, int? page)
        {
            // Get the org
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault(o => o.orgURL == orgURL);

            // If the org doesn't exist, redirect to 404
            if (thisOrg == default(Org))
            {
                Response.Redirect("~/404");
            }

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserVoteCol userVotes = new UserVoteCol();

            if (userId != null)
            {
                // Get the user's votes on this org
                userVotes = countDataService.GetUserVotes(userId);//CountXML<UserVoteCol>.XMLtoDATA();
            }

            ViewBag.userVotes = userVotes;

            // Get the issues of the org
            // TODO: Grab issues chosen by an algorithm based on age and weight
            //IQueryable<Issue> issueList = from issue in dc.Issues
            //                              where issue.OrgId == thisOrg.Id
            //                              orderby issue.votes descending
            //                              select issue;

            var OrgIssues = issueService.GetOrgIssues(thisOrg.Id);

            // MODEL: Put the org and the list of issues into a tuple as our page model
            int num_results_per_page = 3;
            int pageNumber = (page ?? 1);
            Tuple<Org, PagedList.IPagedList<Sigil.Models.Issue>> orgAndIssues = new Tuple<Org, PagedList.IPagedList<Sigil.Models.Issue>>(thisOrg, OrgIssues.ToPagedList(pageNumber, num_results_per_page));

            // This may not actually be necessary.
            //ViewBag.userSub = dc.Subscriptions.SingleOrDefault(s => s.UserId == userId && s.OrgId == thisOrg.Id);

            // Pass our org and issues to the view as the model
            return View(orgAndIssues);
        }

        /* 
        ==================== 
        OrgResponsesPage
  
            Returns a page listing the issues specific to an org with responses, sorted by date descending
        ==================== 
        */

        public ActionResult OrgResponsesPage( string orgURL ) {
            // Get the org
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault( o => o.orgURL == orgURL );

            // If the org doesn't exist, redirect to 404
            if ( thisOrg == default( Org ) ) {
                Response.Redirect( "~/404" );
            }

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserVoteCol userVotes = new UserVoteCol();

            if ( userId != null ) {
                // Get the user's votes on this org
                userVotes = countDataService.GetUserVotes(userId);//CountXML<UserVoteCol>.XMLtoDATA( countDataService.GetUserVotes(userId))//dc.AspNetUsers.Single( u => u.Id == userId ).votes );
            }

            ViewBag.userVotes = userVotes;

            // Get the issues of the org
            // TODO: Grab issues chosen by an algorithm based on age and weight
            //IQueryable<Issue> issueList = from issue in dc.Issues
            //                              where issue.OrgId == thisOrg.Id && issue.responded == true
            //                              select issue;

            var OrgComments = commentService.GetOfficialResponses(thisOrg.Id);

            if (OrgComments.Count > 0) {
                OrgComments.OrderBy( i => i.createTime);
            }

            // MODEL: Put the org and the list of issues into a tuple as our page model
            Tuple<Org, IQueryable<Issue>> orgAndIssues = new Tuple<Org, IQueryable<Issue>>( thisOrg, OrgComments );

            // Again, might not actually be necessary.
            //ViewBag.userSub = dc.Subscriptions.SingleOrDefault( s => s.UserId == userId && s.OrgId == thisOrg.Id );

            // Pass our org and issues to the view as the model
            return View( orgAndIssues );
        }

        /* 
        ==================== 
        OrgData
  
            Data page for an Org, showing views/votes this week/month, responsiveness, and top issues  
        ==================== 
        */

        public JsonResult DefaultData(string orgURL)
        {
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);

            //var orgIssueViews = (from vc in dc.ViewCounts
            //                     where vc.OrgId == thisOrg.Id
            //                     select CountXML<ViewCountCol>.XMLtoDATA(vc.count));

            var orgIssueViewData = countDataService.GetOrgViewCount(thisOrg.Id);

            var View_Data = DataVisualization.Data_to_Google_Graph_Format(orgIssueViewData, DateTime.Now.AddDays(-7), DateTime.Now);

            return Json(View_Data.Select(d => new { viewDate = d.Item1, viewCount = d.Item2 }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CustomData(string orgURL, string dataType, string start, string stop)
        {
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);

            DateTime startDate = DateTimeConversion.FromJSms(start);

            DateTime stopDate = DateTimeConversion.FromJSms(stop);

            List<Tuple<long, int>> view_data = new List<Tuple<long, int>>();

            switch(dataType)
            {
                case "Views":
                    {
                        var data = countDataService.GetOrgViewCount(thisOrg.Id);//dc.ViewCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<ViewCountCol>.XMLtoDATA(v.count)); 
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Votes":
                    {
                        var data = countDataService.GetOrgVoteCount(thisOrg.Id);//dc.VoteCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<VoteCountCol>.XMLtoDATA(v.count));
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Comments":
                    {
                        var data = countDataService.GetOrgCommentCount(thisOrg.Id);//dc.CommentCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<CommentCountCol>.XMLtoDATA(v.count));
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Subscriptions":
                    {
                        var data = countDataService.GetOrgSubscriptionCount(thisOrg.Id);//dc.SubCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<SubCountCol>.XMLtoDATA(v.count));
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "All":
                    {
                        break;
                    }
            }

            return Json(view_data.Select(d => new { viewDate = d.Item1, viewCount = d.Item2 }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrgData(string orgURL)
        {
            // Get the org
            Org thisOrg = orgService.GetOrg(orgURL); //dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);


            /*
             * VIEWBAG
             */

            // Add the org to the ViewBag
            ViewBag.thisOrg = thisOrg;

            // Pass our model list of charts as the model of the view
            // return View(listOfCharts);
            return View();
        }

    }
}