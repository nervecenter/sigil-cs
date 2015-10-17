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


namespace Sigil.Controllers
{
    public class OrgController : Controller
    {
        private SigilDBDataContext dc;

        /* 
        ==================== 
        OrgController
  
            Constructor for our Org controller; creates our persistent data context object 
        ==================== 
        */

        public OrgController()
        {
            dc = new SigilDBDataContext();
        }

        /* 
        ==================== 
        OrgPage
  
            Returns a page listing the latest issues specific to an org 
        ==================== 
        */

        public ActionResult OrgPage(string orgURL, int? page)
        {
            // Get the org
            Org thisOrg = dc.Orgs.FirstOrDefault(o => o.orgURL == orgURL);

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
                userVotes = CountXML<UserVoteCol>.XMLtoDATA(dc.AspNetUsers.Single(u => u.Id == userId).votes);
            }

            ViewBag.userVotes = userVotes;

            // Get the issues of the org
            // TODO: Grab issues chosen by an algorithm based on age and weight
            IQueryable<Issue> issueList = from issue in dc.Issues
                                          where issue.OrgId == thisOrg.Id
                                          orderby issue.votes descending
                                          select issue;

            // MODEL: Put the org and the list of issues into a tuple as our page model
            int num_results_per_page = 3;
            int pageNumber = (page ?? 1);
            Tuple<Org, PagedList.IPagedList<Sigil.Models.Issue>> orgAndIssues = new Tuple<Org, PagedList.IPagedList<Sigil.Models.Issue>>(thisOrg, issueList.ToPagedList(pageNumber, num_results_per_page));

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
            Org thisOrg = dc.Orgs.FirstOrDefault( o => o.orgURL == orgURL );

            // If the org doesn't exist, redirect to 404
            if ( thisOrg == default( Org ) ) {
                Response.Redirect( "~/404" );
            }

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserVoteCol userVotes = new UserVoteCol();

            if ( userId != null ) {
                // Get the user's votes on this org
                userVotes = CountXML<UserVoteCol>.XMLtoDATA( dc.AspNetUsers.Single( u => u.Id == userId ).votes );
            }

            ViewBag.userVotes = userVotes;

            // Get the issues of the org
            // TODO: Grab issues chosen by an algorithm based on age and weight
            IQueryable<Issue> issueList = from issue in dc.Issues
                                          where issue.OrgId == thisOrg.Id && issue.responded == true
                                          select issue;

            if (issueList.Count<Issue>() > 0) {
                issueList.OrderBy( i => i.OfficialResponses.ToList()[ 0 ].createTime );
            }

            // MODEL: Put the org and the list of issues into a tuple as our page model
            Tuple<Org, IQueryable<Issue>> orgAndIssues = new Tuple<Org, IQueryable<Issue>>( thisOrg, issueList );

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

        public ActionResult OrgData(string orgURL)
        {
            // Get the org
            Org thisOrg = dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);


            // MODEL: List of Highcharts to display is out page model
            //List<Highcharts> listOfCharts = new List<Highcharts>();


            // For each day in the week, get that day's views on all issues in the org, group them into a week of integers of views
            var orgIssueViews = (from vc in dc.ViewCounts
                                 where vc.OrgId == thisOrg.Id
                                 select CountXML<ViewCountCol>.XMLtoDATA(vc.count));

            // For each day in the week, get that day's votes on all issues in the org, group them into a week of integers of votes
            var orgIssueVotes = (from vote in dc.VoteCounts
                                 where vote.OrgId == thisOrg.Id
                                 select CountXML<VoteCountCol>.XMLtoDATA(vote.count));

            var orgComments = (from comm in dc.CommentCounts
                               where comm.OrgId == thisOrg.Id
                               select CountXML<CommentCountCol>.XMLtoDATA(comm.count));

            var orgSubs = dc.SubCounts.Where(s => s.OrgId == thisOrg.Id).Select(s => CountXML<SubCountCol>.XMLtoDATA(s.count));

            var allComments = dc.Comments.Where(c => c.Issue.OrgId == thisOrg.Id).Select(c => c);
            var allIssues = dc.Issues.Where(i => i.OrgId == thisOrg.Id).Select(i => i);

            // TODO: Log Org views on Org page

            //========================================= Weekly Traffic Data =======================================================================//




           // var weekChart = DataVisualization.Create_Highchart(orgIssueViews, orgIssueVotes, orgComments, orgSubs, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow, "weekChart", "Traffic on " + thisOrg.orgName + " This Month");


            // Add week chart to our list, get the total counts for views and votes over week, add them and turnover rate to ViewBag
           // listOfCharts.Add(weekChart);
            var weekSums = DataVisualization.Get_Sums(orgIssueViews, orgIssueVotes, orgComments, orgSubs, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow);
            var weekUnique = DataVisualization.Get_Unique_Count(allComments, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow);
            List<Issue> Week_top_issues = DataVisualization.Get_Top_Issues(allIssues, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow, 10);
            List<Issue> Week_under_issues = DataVisualization.Get_Under_Issues(allIssues, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow, 10);

            ViewBag.weekViewCount = weekSums.Item1;
            ViewBag.weekVoteCount = weekSums.Item2;
            ViewBag.weekRatio = ((double)weekSums.Item2 / (double)weekSums.Item1) * 100.0f;
            ViewBag.weekSubCount = weekSums.Item4;
            ViewBag.weekSubRatio = ((double)weekSums.Item4 / (double)weekSums.Item2)*100.0f;
            ViewBag.weekCommCount = weekSums.Item3;
            ViewBag.weekUniqueCommCount = weekUnique;
            ViewBag.weekUniqueRatioViews = ((double)weekUnique / (double)weekSums.Item1) * 100.0f;
            ViewBag.weekUniqueRatioVotes = ((double)weekUnique / (double)weekSums.Item2) * 100.0f;
            ViewBag.weekTopIssues = Week_top_issues;
            ViewBag.weekUnderDogIssues = Week_under_issues;



        //========================================= MONTHLY Traffic Data =======================================================================//

            // Create a Highchart with X-axis for days of the month, and Y-axis series logging views and votes
           // Highcharts monthChart = DataVisualization.Create_Highchart(orgIssueViews, orgIssueVotes, orgComments, orgSubs, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, "monthChart", "Traffic on " + thisOrg.orgURL + " This Month");

            // Add month chart to our list, get the total counts for views and votes over month, add them and turnover rate to ViewBag
          //  listOfCharts.Add(monthChart);
            var monthSums = DataVisualization.Get_Sums(orgIssueViews, orgIssueVotes, orgComments, orgSubs, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
            var monthUnique = DataVisualization.Get_Unique_Count(allComments, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
            List<Issue> Month_top_issues = DataVisualization.Get_Top_Issues(allIssues, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, 10);
            List<Issue> Month_under_issues = DataVisualization.Get_Under_Issues(allIssues, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, 10);



            ViewBag.monthViewCount = monthSums.Item1;
            ViewBag.monthVoteCount = monthSums.Item2;
            ViewBag.monthRatio = ((double)monthSums.Item2 / (double)monthSums.Item1) * 100.0f;
            ViewBag.monthSubCount = monthSums.Item4;
            ViewBag.monthSubRatio = ((double)monthSums.Item4 / (double)monthSums.Item2) * 100.0f; ;
            ViewBag.monthCommCount = monthSums.Item3;
            ViewBag.monthUniqueCommCount = monthUnique ;
            ViewBag.monthUniqueRatioViews = ((double)monthUnique / (double)monthSums.Item1) * 100.0f; 
            ViewBag.monthUniqueRatioVotes = ((double)monthUnique / (double)monthSums.Item2) * 100.0f; ;
            ViewBag.monthTopIssues = Month_top_issues;
            ViewBag.monthUnderDogIssues = Month_under_issues;
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