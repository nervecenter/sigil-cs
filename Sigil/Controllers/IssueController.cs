using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;


namespace Sigil.Controllers
{
    public class IssueController : Controller
    {
        // GET: Issue
               private SigilDBDataContext dc;

        /* 
        ==================== 
        IssueController
  
            Constructor for our Issue controller; creates our persistent data context object 
        ==================== 
        */

        public IssueController() {
            dc = new SigilDBDataContext();
        }

        /* 
        ==================== 
        IssuePage
  
            The main page for an issue in any org. Contains vote buttons, issue text, responses, comment section. 
        ==================== 
        */

        public ActionResult IssuePage( string orgName, long issueID ) {
            // Grab the issue's org
            Org thisOrg = dc.Orgs.SingleOrDefault<Org>(o => o.orgName == orgName);

            // Grab the issue for the page
            Issue thisIssue = (from issue in dc.Issues
                               where issue.Id == issueID
                               select issue).SingleOrDefault();

            // If neither are valid, redirect to 404 page
            if ( thisOrg == default(Org) || thisIssue == default(Issue) ) {
                Response.Redirect("~/404");
            }

            // Get the user and their subscriptions
            var userID = User.Identity.GetUserId();
            if (userID != null)
            {

                // If the page is in POST, we're posting a comment; get form data and POST it
                if (Request.HttpMethod == "POST")
                {
                    // Create a new issue
                    Comment newComment = new Comment();

                    // Increment Id, drop in current user and date, set default weight, drop in the form text
                    //newComment.Id = dc.Comments.Max<Comment>(c => c.Id) + 1;
                    newComment.issueId = thisIssue.Id;
                    newComment.UserId = userID;
                    newComment.postDate = DateTime.UtcNow;
                    newComment.editTime = DateTime.UtcNow;
                    newComment.lastVoted = DateTime.UtcNow;
                    newComment.votes = 1;

                    newComment.text = Request.Form["text"];
                    
                    // Try to submit the issue and go to the issue page; otherwise, write an error
                    try
                    {
                        dc.Comments.InsertOnSubmit(newComment);

                        dc.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not write comment from %s to database:\n%s", newComment.UserId, e.Message);
                    }
                }


                // Get the user's vote on this issues if it exists
                Vote userVote = (from vote in dc.Votes
                                 where vote.UserID == userID && vote.IssueId == thisIssue.Id
                                 select vote).SingleOrDefault<Vote>();
                // Grab the issue's comments sorted by date

                ViewBag.userVote = userVote;
            }
            //// Get today's issue viewcount and increment it; if there is none for today, create a new table entry for it
            ViewCount vc = dc.ViewCounts.FirstOrDefault<ViewCount>(v => v.IssueId == thisIssue.Id && v.datetime.Date == DateTime.Today.Date);
            if (vc == default(ViewCount))
            {
                try
                {
                    vc = new ViewCount();
                    vc.Id = dc.ViewCounts.Max<ViewCount>(v => v.Id) + 1;
                    vc.datetime = DateTime.UtcNow;
                    vc.OrgId = thisOrg.Id;
                    vc.IssueId = thisIssue.Id;
                    vc.count = 1;
                    dc.ViewCounts.InsertOnSubmit(vc);
                    dc.SubmitChanges();
                }
                catch
                {
                    Console.WriteLine("Could not add new view count object issue %d.", thisIssue.Id);
                }
            }
            else
            {
                try
                {
                    vc.count++;
                    dc.SubmitChanges();
                }
                catch
                {
                    Console.WriteLine("Could not update view count on issue %d.", thisIssue.Id);
                }
            }

            IQueryable<Comment> issueComments = from comment in dc.Comments
                                where comment.issueId == thisIssue.Id
                                orderby comment.postDate descending
                                select comment;

            // MODEL: A tuple of the org and the issue are the model for the IssuePage
            Tuple<Org, Issue, IQueryable<Comment>> orgIssueComments = new Tuple<Org,Issue, IQueryable<Comment>>( thisOrg, thisIssue, issueComments );

            // Add the user and their vote (or lack of vote) to the ViewBag



            // Pass the org and issue as the model to the view
            return View( orgIssueComments );
        }


        /* 
        ==================== 
        IssueData
  
            Data page for an issue, showing views/votes this week/month 
        ==================== 
        */

        public ActionResult IssueData( string orghandle, long issueID ) {
            // Get the issue
            Issue thisIssue = (from issue in dc.Issues
                               where issue.Id == issueID
                               select issue).SingleOrDefault();

            // Get the org
            Org thisOrg = thisIssue.Org;

            // If neither are valid, redirect to 404 page
            if ( thisOrg == default(Org) || thisIssue == default(Issue) ) {
                Response.Redirect("~/404");
            }

            // MODEL: List of charts we'll be displaying in sequence
            List<Highcharts> listOfCharts = new List<Highcharts>();

            /*
             *  WEEKLY Traffic Data
             */

            // TODO: Dynamically fill in holes properly where no votes or views have happened, rather than appending zeros to the end

            // For each day in the week, get that day's views on the issue, group them into a week of integers of views
            List<int> weekOfViews = (from vc in dc.ViewCounts
                                     where vc.IssueId == thisIssue.Id && vc.datetime.Date >= DateTime.Today.Date.AddDays(-6.0)
                                     orderby vc.datetime descending
                                     select vc.count).ToList<int>();

            // If there were days without entries (no views), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no views
            while ( weekOfViews.Count < 7 ) {
                weekOfViews.Add(0);
            }

            // For each day in the week, get that day's votes on the issue in the org, group them into a week of integers of votes
            List<int> weekOfVotes = (from vote in dc.Votes
                                     where vote.IssueId == thisIssue.Id && vote.voteDate.Date >= DateTime.Today.Date.AddDays(-6.0)
                                     group vote by vote.voteDate.Date into day
                                     select day.Count()).ToList<int>();

            // If there were days without entries (no votes), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no votes
            while ( weekOfVotes.Count < 7 ) {
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
                .SetTitle(new Title{ Text = "Traffic on Issue " + thisIssue.Id + " This Week" });

            // Add week chart to our list, get the total counts for views and votes over week, add them and turnover rate to ViewBag
            listOfCharts.Add(weekChart);
            int weekViewCount = weekOfViews.Sum();
            int weekVoteCount = weekOfVotes.Sum();
            ViewBag.weekViewCount = weekViewCount;
            ViewBag.weekVoteCount = weekVoteCount;
            ViewBag.weekRatio = ( (double)weekVoteCount / (double)weekViewCount ) * 100.0f;


            /*
             *  MONTHLY Traffic Data
             */

            // For each day in the month, get that day's views on the issue, group them into a month of integers of views
            List<int> monthOfViews = (from vc in dc.ViewCounts
                                      where vc.IssueId == thisIssue.Id && vc.datetime.Date >= DateTime.Today.Date.AddDays(-29.0)
                                      orderby vc.datetime descending
                                      select vc.count).ToList<int>();

            // If there were days without entires (no views), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no views
            while ( monthOfViews.Count < 30 ) {
                monthOfViews.Add(0);
            }

            // For each day in the month, get that day's votes on the issue, group them into a month of integers of votes
            List<int> monthOfVotes = (from vote in dc.Votes
                                      where vote.IssueId == thisIssue.Id && vote.voteDate.Date >= DateTime.Today.Date.AddDays(-29.0)
                                      group vote by vote.voteDate.Date into day
                                      select day.Count()).ToList<int>();

            // If there were days without entires (no votes), append zeros to the beginning
            // TODO: Fix this such that it inserts zeros into the days with no votes
            while ( monthOfVotes.Count < 30 ) {
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
                                Labels = new XAxisLabels {
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
                .SetTitle(new Title{ Text = "Traffic on Issue " + thisIssue.Id + " This Month" });

            // Add month chart to our list, get the total counts for views and votes over month, add them and turnover rate to ViewBag
            listOfCharts.Add(monthChart);
            int monthViewCount = monthOfViews.Sum();
            int monthVoteCount = monthOfVotes.Sum();
            ViewBag.monthViewCount = monthViewCount;
            ViewBag.monthVoteCount = monthVoteCount;
            ViewBag.monthRatio = ( (double)monthVoteCount / (double)monthViewCount ) * 100.0f;


            /*
             * VIEWBAG
             */

            // Add the issue and org to the ViewBag
            ViewBag.thisIssue = thisIssue;
            ViewBag.thisOrg = thisOrg;

            // Pass our model list of charts as the model of the view
            return View(listOfCharts);
        }

        /* 
        ==================== 
        AddIssue
  
            Form for adding an issue. Linked on org page. Adds issue to current org. 
        ==================== 
        */
        [Authorize]
        public ActionResult AddIssue( string orgName ) {
            // Get the org for the issue we're adding
            Org thisOrg = dc.Orgs.First<Org>(o => o.orgName == orgName);

            // Get the user
            var userId = User.Identity.GetUserId();

            // If the page is in POST, get the issue form data and POST it
            if ( Request.HttpMethod == "POST" ) {
                // Create a new issue
                Issue newissue = new Issue();
                
                // Increment Id, drop in current user and date, set default weight, drop in the form text

                newissue.Org = thisOrg;
                newissue.OrgId = thisOrg.Id;
                newissue.UserId = userId;
                newissue.createTime = DateTime.UtcNow;
                newissue.editTime = DateTime.UtcNow;
                newissue.lastVoted = DateTime.UtcNow;
                newissue.votes = 1;
                newissue.viewCount = 1;
                newissue.title = Request.Form["title"];
                newissue.text = Request.Form[ "text" ];
                // Try to submit the issue and go to the issue page; otherwise, write an error
                try {
                    dc.Issues.InsertOnSubmit( newissue );
                    dc.SubmitChanges();
                    var lastId = dc.Issues.Max<Issue>(i => i.Id);
                    Response.Redirect( "~/" + thisOrg.orgName + "/" + lastId );
                } catch ( Exception e ) {
                    Console.WriteLine( "Could not write issue \"%s\" to database:\n%s", newissue.text, e.Message );
                }
            }

            // Add the org to the ViewBag
            ViewBag.thisOrg = thisOrg;

            return View();
        }

        /* 
        ==================== 
        VoteUp
  
            Action called by AJAX on click of a vote up button. Does not create a page, only increments vote counter and adds vote table entry. 
        ==================== 
        */
        [Authorize]
        public ActionResult VoteUp( int issueID) {
            // Find our issue object and create a new vote
            Issue thisIssue = dc.Issues.First<Issue>( i => i.Id == issueID );
            Vote thisVote = new Vote();
            var userId = User.Identity.GetUserId();
            // Increment issue's vote counter, initialize our new vote for the issue/user, POST both to server; otherwise, log an error
            try {
                thisIssue.votes++;

                
                thisVote.voteDate = DateTime.UtcNow;
                thisVote.IssueId = thisIssue.Id;
                thisVote.UserID = userId;
                
                dc.Votes.InsertOnSubmit( thisVote );
                dc.SubmitChanges();
            } catch ( Exception e ) {
                Console.WriteLine( "Could not vote on issue %s:\n%s", thisIssue.Id, e.Message );
            }
            return new EmptyResult();
        }

        /* 
        ==================== 
        UnVoteUp
  
            Action called by AJAX on click of a unvote up button. Does not create a page, only decrements vote counter and deletes vote table entry. 
        ==================== 
        */
        [Authorize]
        public ActionResult UnVoteUp( int issueID) {
            // Find our issue object and vote object
            Issue thisIssue = dc.Issues.First<Issue>(i => i.Id == issueID);
            var userId = User.Identity.GetUserId();

            Vote thisVote = dc.Votes.First<Vote>( v => v.UserID == userId && v.IssueId == thisIssue.Id );

            // Decrement vote counter for issue, delete vote entry, POST changes to server; otherwise, log an error
            try {
                thisIssue.votes--;

                dc.Votes.DeleteOnSubmit( thisVote );
                dc.SubmitChanges();
            } catch ( Exception e ) {
                Console.WriteLine( "Could not unvote on issue %s:\n%s", thisIssue.Id, e.Message );
            }
            return new EmptyResult();
        }
    }
}
