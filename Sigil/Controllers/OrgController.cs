using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using Sigil.Models;

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

        public ActionResult OrgPage(string orgURL)
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
            IQueryable<Vote> userVotes;

            if (userId != null)
            {
                // Get the user's votes on this org
                userVotes = from vote in dc.Votes
                            where vote.UserID == userId && vote.Issue.OrgId == thisOrg.Id
                            select vote;
                ViewBag.userVotes = userVotes;
            }
            // Get the issues of the org
            // TODO: Grab issues chosen by an algorithm based on age and weight
            IQueryable<Issue> issueList = from issue in dc.Issues
                                          where issue.OrgId == thisOrg.Id
                                          orderby issue.votes descending
                                          select issue;

            // MODEL: Put the org and the list of issues into a tuple as our page model
            Tuple<Org, IQueryable<Issue>> orgAndIssues = new Tuple<Org, IQueryable<Issue>>(thisOrg, issueList);

            ViewBag.userSub = dc.Subscriptions.SingleOrDefault( s => s.UserId == userId && s.OrgId == thisOrg.Id );

            // Pass our org and issues to the view as the model
            return View(orgAndIssues);
        }

        /* 
        ==================== 
        OrgSubscribe
  
            Action method to be linked for allowing a user to subscribe to an org  
        ==================== 
        
        [Authorize]
        public ActionResult OrgSubscribe( string orgUrl ) {
            Subscription newSub = new Subscription();
            string userId = User.Identity.GetUserId();
            Org thisOrg = dc.Orgs.Single( o => o.orgURL == orgUrl );

            try {
                newSub.UserId = userId;
                newSub.OrgId = thisOrg.Id;

                dc.Subscriptions.InsertOnSubmit( newSub );
                dc.SubmitChanges();
            } catch ( Exception e ) {
                Console.WriteLine( "Could not subscribe to org: User %s, Org %s\n%s", User.Identity.GetUserName(), thisOrg.orgURL, e.Message );
            }
            return new EmptyResult();
        }

        /* 
        ==================== 
        OrgUnSubscribe
  
            Action method to be linked for allowing a user to unsubscribe from an org  
        ==================== 
        
        [Authorize]
        public ActionResult OrgUnSubscribe( string orgUrl ) {
            string userId = User.Identity.GetUserId();
            Subscription thisSub = dc.Subscriptions.Single( s => s.UserId == userId );

            try {
                dc.Subscriptions.DeleteOnSubmit( thisSub );
                dc.SubmitChanges();
            } catch ( Exception e ) {
                Console.WriteLine( "Could not unsubscribe from org: User %s, Org %s\n%s", User.Identity.GetUserName(), thisSub.Org.orgURL, e.Message );
            }
            return new EmptyResult();
        }*/

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
            List<Highcharts> listOfCharts = new List<Highcharts>();

            // TODO: Log Org views on Org page

            /*
             *  WEEKLY Traffic Data
             */

            // For each day in the week, get that day's views on all issues in the org, group them into a week of integers of views
            List<int> weekOfViews = (from vc in dc.ViewCounts
                                     where vc.Issue.OrgId == thisOrg.Id && vc.datetime.Date >= DateTime.Today.Date.AddDays(-6.0)
                                     group vc by vc.datetime.Date into day
                                     select day.Sum<ViewCount>(v => v.count)).ToList<int>();

            // If there were days without entries (no views), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no views
            while (weekOfViews.Count < 7)
            {
                weekOfViews.Add(0);
            }

            // For each day in the week, get that day's votes on all issues in the org, group them into a week of integers of votes
            List<int> weekOfVotes = (from vote in dc.Votes
                                     where vote.Issue.OrgId == thisOrg.Id && vote.voteDate.Date >= DateTime.Today.Date.AddDays(-6.0)
                                     group vote by vote.voteDate.Date into day
                                     select day.Count()).ToList<int>();

            // If there were days without entries (no votes), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no votes
            while (weekOfVotes.Count < 7)
            {
                weekOfVotes.Add(0);
            }

            // TODO: Possible solution: For each day up to current, add datestring to array, and add votecount to array; if no entry, add 0

            // Create a Highchart with X-axis for days of the week, and Y-axis series logging views and votes
            Highcharts weekChart = new DotNet.Highcharts.Highcharts("weekChart")
                .SetXAxis(new XAxis
                {
                    Categories = new[] { DateTime.Today.Date.AddDays(-6.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-5.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-4.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-3.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-2.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-1.0).ToShortDateString(), 
                                                     DateTime.Today.Date.ToShortDateString() }
                })
                .SetSeries(new Series[] {
                            new Series {
                                Data = new Data(new object[] { weekOfViews[6], 
                                                               weekOfViews[5], 
                                                               weekOfViews[4], 
                                                               weekOfViews[3], 
                                                               weekOfViews[2], 
                                                               weekOfViews[1], 
                                                               weekOfViews[0] }),
                                Name = "Views"
                            }, new Series {
                                Data = new Data(new object[] { weekOfVotes[6], 
                                                               weekOfVotes[5], 
                                                               weekOfVotes[4], 
                                                               weekOfVotes[3], 
                                                               weekOfVotes[2], 
                                                               weekOfVotes[1], 
                                                               weekOfVotes[0] }),
                                Name = "Votes"
                            }
                })
                .SetTitle(new Title { Text = "Traffic on " + thisOrg.orgURL+ " This Month" });

            // Add week chart to our list, get the total counts for views and votes over week, add them and turnover rate to ViewBag
            listOfCharts.Add(weekChart);
            int weekViewCount = weekOfViews.Sum();
            int weekVoteCount = weekOfVotes.Sum();
            ViewBag.weekViewCount = weekViewCount;
            ViewBag.weekVoteCount = weekVoteCount;
            ViewBag.weekRatio = ((double)weekVoteCount / (double)weekViewCount) * 100.0f;

            /*
             *  MONTHLY Traffic Data
             */

            // For each day in the month, get that day's views on all issues in the org, group them into a month of integers of views
            List<int> monthOfViews = (from vc in dc.ViewCounts
                                      where vc.Issue.OrgId == thisOrg.Id && vc.datetime.Date >= DateTime.Today.Date.AddDays(-29.0)
                                      group vc by vc.datetime.Date into day
                                      select day.Sum<ViewCount>(v => v.count)).ToList<int>();

            // If there were days without entires (no views), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no views
            while (monthOfViews.Count < 30)
            {
                monthOfViews.Add(0);
            }

            // For each day in the month, get that day's votes on all issues in the org, group them into a month of integers of votes
            List<int> monthOfVotes = (from vote in dc.Votes
                                      where vote.Issue.OrgId == thisOrg.Id && vote.voteDate.Date >= DateTime.Today.Date.AddDays(-29.0)
                                      group vote by vote.voteDate.Date into day
                                      select day.Count()).ToList<int>();

            // If there were days without entires (no votes), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no votes
            while (monthOfVotes.Count < 30)
            {
                monthOfVotes.Add(0);
            }

            // TODO: Possible solution: For each day up to current, add datestring to array, and add votecount to array; if no entry, add 0

            // Create a Highchart with X-axis for days of the month, and Y-axis series logging views and votes
            Highcharts monthChart = new DotNet.Highcharts.Highcharts("monthChart")
                .SetXAxis(new XAxis
                {
                    Categories = new[] { DateTime.Today.Date.AddDays(-29.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-28.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-27.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-26.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-25.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-24.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-23.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-22.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-21.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-20.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-19.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-18.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-17.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-16.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-15.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-14.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-13.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-12.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-11.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-10.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-9.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-8.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-7.0).ToShortDateString(),
                                                     DateTime.Today.Date.AddDays(-6.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-5.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-4.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-3.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-2.0).ToShortDateString(), 
                                                     DateTime.Today.Date.AddDays(-1.0).ToShortDateString(), 
                                                     DateTime.Today.Date.ToShortDateString() },
                    Labels = new XAxisLabels
                    {
                        Rotation = -45
                    }
                })
                .SetSeries(new Series[] {
                            new Series {
                                Data = new Data(new object[] { monthOfViews[29], 
                                                               monthOfViews[28],
                                                               monthOfViews[27], 
                                                               monthOfViews[26], 
                                                               monthOfViews[25], 
                                                               monthOfViews[24], 
                                                               monthOfViews[23], 
                                                               monthOfViews[22], 
                                                               monthOfViews[21],
                                                               monthOfViews[20], 
                                                               monthOfViews[19], 
                                                               monthOfViews[18], 
                                                               monthOfViews[17], 
                                                               monthOfViews[16], 
                                                               monthOfViews[15], 
                                                               monthOfViews[14],
                                                               monthOfViews[13], 
                                                               monthOfViews[12], 
                                                               monthOfViews[11], 
                                                               monthOfViews[10], 
                                                               monthOfViews[9], 
                                                               monthOfViews[8], 
                                                               monthOfViews[7],
                                                               monthOfViews[6], 
                                                               monthOfViews[5], 
                                                               monthOfViews[4], 
                                                               monthOfViews[3], 
                                                               monthOfViews[2], 
                                                               monthOfViews[1], 
                                                               monthOfViews[0] }),
                                Name = "Views"
                            }, new Series {
                                Data = new Data(new object[] { monthOfVotes[29], 
                                                               monthOfVotes[28],
                                                               monthOfVotes[27], 
                                                               monthOfVotes[26], 
                                                               monthOfVotes[25], 
                                                               monthOfVotes[24], 
                                                               monthOfVotes[23], 
                                                               monthOfVotes[22], 
                                                               monthOfVotes[21],
                                                               monthOfVotes[20], 
                                                               monthOfVotes[19], 
                                                               monthOfVotes[18], 
                                                               monthOfVotes[17], 
                                                               monthOfVotes[16], 
                                                               monthOfVotes[15], 
                                                               monthOfVotes[14],
                                                               monthOfVotes[13], 
                                                               monthOfVotes[12], 
                                                               monthOfVotes[11], 
                                                               monthOfVotes[10], 
                                                               monthOfVotes[9], 
                                                               monthOfVotes[8], 
                                                               monthOfVotes[7],
                                                               monthOfVotes[6], 
                                                               monthOfVotes[5], 
                                                               monthOfVotes[4], 
                                                               monthOfVotes[3], 
                                                               monthOfVotes[2], 
                                                               monthOfVotes[1], 
                                                               monthOfVotes[0] }),
                                Name = "Votes"
                            }
                })
                .SetTitle(new Title { Text = "Traffic on " + thisOrg.orgURL + " This Month" });

            // Add month chart to our list, get the total counts for views and votes over month, add them and turnover rate to ViewBag
            listOfCharts.Add(monthChart);
            int monthViewCount = monthOfViews.Sum();
            int monthVoteCount = monthOfVotes.Sum();
            ViewBag.monthViewCount = monthViewCount;
            ViewBag.monthVoteCount = monthVoteCount;
            ViewBag.monthRatio = ((double)monthVoteCount / (double)monthViewCount) * 100.0f;


            /*
             * VIEWBAG
             */

            // Add the org to the ViewBag
            ViewBag.thisOrg = thisOrg;

            // Pass our model list of charts as the model of the view
            return View(listOfCharts);
        }
    }
}