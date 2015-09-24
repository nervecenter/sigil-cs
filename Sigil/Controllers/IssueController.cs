﻿using System;
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
using System.Data.SqlTypes;

namespace Sigil.Controllers
{
    enum VoteState {
        NotLoggedIn,
        Voted,
        NotVoted
    }

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
        public ActionResult IssuePage( string orgURL, long issueID )
        {
            // Grab the issue's org
            Org thisOrg = dc.Orgs.SingleOrDefault<Org>(o => o.orgURL == orgURL);

            // Grab the issue for the page
            Issue thisIssue = (from issue in dc.Issues
                               where issue.Id == issueID
                               select issue).SingleOrDefault();

            // If neither are valid, redirect to 404 page
            if (thisOrg == default(Org) || thisIssue == default(Issue))
            {
                Response.Redirect("~/404");
            }

            //update the total viewcount of the issue
            Update_View_Count(thisIssue);

            //// Update the viewcount for this day specifically
            ViewCount_Routine(thisOrg, thisIssue);

            // Get the user
            var userID = User.Identity.GetUserId();

            // If the page is in POST, we're posting a comment; get form data and POST it
            if (Request.HttpMethod == "POST")
            {
                // Create a new issue
                Create_New_Comment(thisIssue, userID);
            }

            // Get the user's vote on this issues if it exists
            AspNetUser user = dc.AspNetUsers.SingleOrDefault(u => u.Id == userID);
            UserVoteCol userVote = (user != default(AspNetUser)) ? CountXML<UserVoteCol>.XMLtoDATA(user.votes) : new UserVoteCol();

            ViewBag.userVote = userVote;



            IQueryable<Comment> issueComments = from comment in dc.Comments
                                                where comment.issueId == thisIssue.Id
                                                orderby comment.postDate descending
                                                select comment;

            // MODEL: A tuple of the org and the issue are the model for the IssuePage
            Tuple<Org, Issue, IQueryable<Comment>> orgIssueComments = new Tuple<Org, Issue, IQueryable<Comment>>(thisOrg, thisIssue, issueComments);

            // Pass the org and issue as the model to the view
            return View(orgIssueComments);
        }

        /// <summary>
        /// Increments the issues ViewCount table entry or if not created yet, makes a new viewcount entry for the issue.
        /// </summary>
        /// <param name="thisOrg">Org of the passed in issue</param>
        /// <param name="thisIssue">Issue object from the table</param>
        private void ViewCount_Routine(Org thisOrg, Issue thisIssue)
        {
            ViewCount vc = dc.ViewCounts.FirstOrDefault<ViewCount>(v => v.IssueId == thisIssue.Id);//&& v.datetime.Date == DateTime.Today.Date);
            if (vc == default(ViewCount))
            {
                try
                {
                    vc = new ViewCount();
                    vc.OrgId = thisOrg.Id;
                    vc.IssueId = thisIssue.Id;
                    vc.count = CountXML<ViewCountCol>.DATAtoXML(new ViewCountCol());
                    dc.ViewCounts.InsertOnSubmit(vc);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    //WRITE TO ERROR FILE
                    ErrorHandler.Log_Error(vc, e);
                    //Console.WriteLine("Could not add new view count object issue %d.", vc, e.Message);
                }
            }
            else
            {
                try
                {
                    var updatedVC = CountXML<ViewCountCol>.XMLtoDATA(vc.count);
                    updatedVC.Update();
                    vc.count = CountXML<ViewCountCol>.DATAtoXML(updatedVC);
                    dc.SubmitChanges();
                }
                catch(Exception e)
                {
                    //WRITE TO ERROR FILE
                    ErrorHandler.Log_Error(thisIssue, e);

                }
            }
        }

        /// <summary>
        /// Creates a new comment and saves to DB
        /// </summary>
        /// <param name="thisIssue">Issue the comment is for</param>
        /// <param name="userID">Comment creators user ID</param>
        private void Create_New_Comment(Issue thisIssue, string userID)
        {
            Comment newComment = new Comment();
            
            // Increment Id, drop in current user and date, set default weight, drop in the form text
            newComment.issueId = thisIssue.Id;
            newComment.UserId = userID;
            newComment.postDate = DateTime.UtcNow;
            newComment.editTime = DateTime.UtcNow;
            newComment.lastVoted = DateTime.UtcNow;
            newComment.votes = 1;

            newComment.text = Request.Form["text"];
            Notification_Check(newComment.text, userID);

            var commentData = dc.CommentCounts.Single(c => c.OrgId == thisIssue.OrgId && c.IssueId == thisIssue.Id);

            var commDataCol = CountXML<CommentCountCol>.XMLtoDATA(commentData.count);
            commDataCol.Update();
            commentData.count = CountXML<CommentCountCol>.DATAtoXML(commDataCol);

            // Try to submit the issue and go to the issue page; otherwise, write an error
            try
            {
                dc.Comments.InsertOnSubmit(newComment);

                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                //WRITE TO ERROR FILE
                ErrorHandler.Log_Error(newComment, e);
                ErrorHandler.Log_Error(commentData, e);

            }
        }

        /// <summary>
        /// Updates the total view count of the passed in issue.
        /// </summary>
        /// <param name="issue"></param>
        private void Update_View_Count(Issue issue)
        {
            try
            {
                issue.viewCount++;
                dc.SubmitChanges();

            }
            catch (Exception e)
            {
                //WILL WRITE TO ERROR FILE
                ErrorHandler.Log_Error(issue, e);

            }

        }

        /* 
        ==================== 
        IssueData
  
            Data page for an issue, showing views/votes this week/month 
        ==================== 
        */
        [Authorize]
        public ActionResult IssueData(string orghandle, long issueID)
        {
            // Get the issue
            Issue thisIssue = dc.Issues.SingleOrDefault(i => i.Id == issueID);

            // Get the org
            Org thisOrg = thisIssue.Org;

            // If neither are valid, redirect to 404 page
            if (thisOrg == default(Org) || thisIssue == default(Issue))
            {
                Response.Redirect("~/404");
            }

            // MODEL: List of charts we'll be displaying in sequence
            List<Highcharts> listOfCharts = new List<Highcharts>();

            //get the collection of viewcounts from the xml field in the table
            ViewCountCol issueViews = CountXML<ViewCountCol>.XMLtoDATA(dc.ViewCounts.Single(v => v.IssueId == thisIssue.Id && v.OrgId == thisIssue.OrgId).count);
            VoteCountCol issueVotes = CountXML<VoteCountCol>.XMLtoDATA(dc.VoteCounts.Single(v => v.IssueId == thisIssue.Id && v.OrgId == thisIssue.OrgId).count);
            CommentCountCol issueComms = CountXML<CommentCountCol>.XMLtoDATA(dc.CommentCounts.Single(c => c.IssueId == thisIssue.Id && c.OrgId == thisIssue.OrgId).count);
            
            /*
             *  WEEKLY Traffic Data
             */
            
            Highcharts weekChart = DataVisualization.Create_Highchart(issueViews, issueVotes, issueComms,DateTime.UtcNow, DateTime.UtcNow.AddDays(-6),"weekchart", "Traffic on Issue " + thisIssue.Id + " This Week");

            // Add week chart to our list, get the total counts for views and votes over week, add them and turnover rate to ViewBag
            listOfCharts.Add(weekChart);
            var totals = DataVisualization.Get_Sums(issueViews, issueVotes, issueComms, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow);
            var uniqueCommsWeek = DataVisualization.Get_Unique_Count(dc.Comments.Where(c => c.issueId == thisIssue.Id).Select(c => c), DateTime.UtcNow.AddDays(-6), DateTime.UtcNow);
            ViewBag.weekViewCount = totals.Item1;
            ViewBag.weekVoteCount = totals.Item2;
            ViewBag.weekCommCount = totals.Item3;
            ViewBag.weekCommUnique = uniqueCommsWeek;
            ViewBag.weekRatio = ((double)totals.Item2 / (double)totals.Item1) * 100.0f;
            ViewBag.weekUniqueRatioViews = ((double)uniqueCommsWeek / (double)totals.Item1) * 100.0f;
            ViewBag.weekUniqueRatioVotes = ((double)uniqueCommsWeek / (double)totals.Item1) * 100.0f;

            /*
             *  MONTHLY Traffic Data
             */


            // Create a Highchart with X-axis for days of the month, and Y-axis series logging views and votes
            Highcharts monthChart = DataVisualization.Create_Highchart(issueViews, issueVotes, issueComms, DateTime.UtcNow, DateTime.UtcNow.AddMonths(-1), "monthchart", "Traffic on Issue " + thisIssue.Id + " This Month");

            // Add month chart to our list, get the total counts for views and votes over month, add them and turnover rate to ViewBag
            listOfCharts.Add(monthChart);
            totals = DataVisualization.Get_Sums(issueViews, issueVotes, issueComms,DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
            var uniqueCommsMonth = DataVisualization.Get_Unique_Count(dc.Comments.Where(c => c.issueId == thisIssue.Id).Select(c => c), DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
            ViewBag.monthViewCount = totals.Item1;
            ViewBag.monthVoteCount = totals.Item2;
            ViewBag.monthCommCount = totals.Item3;
            ViewBag.monthCommUnique = uniqueCommsMonth;
            ViewBag.monthRatio = ((double)totals.Item2 / (double)totals.Item1) * 100.0f;
            ViewBag.monthUniqueRatioViews = ((double)uniqueCommsMonth / (double)totals.Item1) * 100.0f;
            ViewBag.monthUniqueRatioVotes = ((double)uniqueCommsMonth / (double)totals.Item1) * 100.0f;


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
        public ActionResult CreateIssue()
        {
            return View("AddIssue");
        }


        [HttpPost]
        [Authorize]
        public ActionResult AddIssue()
        {
            // Get the org for the issue we're adding

            // Get the user
            var userId = User.Identity.GetUserId();

            // check to see if they are posting to a specific org's category or just to the org itself
            string orgName = Request.Form["orgName"];
            string category = null;
            if (orgName.Contains("-"))
            {
                var temp = orgName.Split('-');
                orgName = temp[0];
                category = temp[1];
            }

            var org = dc.Orgs.Where(o => o.orgName == orgName).Single();
            var catid = dc.Categories.SingleOrDefault(c => c.catName == category && c.orgId == org.Id);

            //pass in creators userid, the org and possible category of the issue
            int issueId = Create_New_Issue(userId, org, catid);

            if (issueId == 0)
            {
                //this is where we need to redirect to a page if the issue posting failed in the previous function call
            }

            New_Issue_Vote_Routine(userId, org, issueId);

            return Redirect("~/" + org.orgURL + "/" + issueId);

        }

        private void New_Issue_Vote_Routine(string userId, Org org, int issueId)
        {
            //Create Vote count table entry
            VoteCount newVote = new VoteCount();
            newVote.IssueId = issueId;
            newVote.OrgId = org.Id;

            VoteCountCol newVoCol = new VoteCountCol();
            newVoCol.Update();
            newVote.count = CountXML<VoteCountCol>.DATAtoXML(newVoCol);

            //Create Comment count table entry
            CommentCount newCom = new CommentCount();
            newCom.IssueId = issueId;
            newCom.OrgId = org.Id;
            newCom.count = CountXML<CommentCountCol>.DATAtoXML(new CommentCountCol());
            
            //update the users votes xml ds because every user votes on the issue they make
            var user = dc.AspNetUsers.Single(u => u.Id == userId);
            var userVotes = CountXML<UserVoteCol>.XMLtoDATA(user.votes);
            userVotes.Add_Vote(issueId, org.Id);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVotes);

            try
            {
                dc.VoteCounts.InsertOnSubmit(newVote);
                dc.CommentCounts.InsertOnSubmit(newCom);
                dc.SubmitChanges();

            }
            catch (Exception e)
            {
                ErrorHandler.Log_Error(newVote, e);
                //Console.WriteLine("Could not write vote \"%s\" to database:\n%s", newVote, e.Message);
            }
        }

        private int Create_New_Issue(string userId, Org org, Category catid)
        {
            // Create a new issue
            Issue newissue = new Issue();

            // Increment Id, drop in current user and date, set default weight, drop in the form text
            newissue.Org = org;
            newissue.OrgId = org.Id;
            newissue.UserId = userId;
            newissue.createTime = DateTime.UtcNow;
            newissue.editTime = DateTime.UtcNow;
            newissue.lastVoted = DateTime.UtcNow;
            newissue.votes = 1;
            newissue.viewCount = 1;
            newissue.title = Request.Form["title"];
            newissue.text = Request.Form["text"];
            newissue.CatId = catid == default(Category) ? 0 : catid.Id;
            newissue.TopicId = catid == default(Category) ? org.topicid : catid.topicId;
            if (catid != null)
                newissue.CatId = catid.Id;
            // Try to submit the issue and go to the issue page; otherwise, write an error
            try
            {
                dc.Issues.InsertOnSubmit(newissue);
                dc.SubmitChanges();

                return dc.Issues.Last(i => i.UserId == userId && i.OrgId == org.Id).Id;


            }
            catch (Exception e)
            {
                ErrorHandler.Log_Error(newissue, e);
                return 0;
            }
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
            Org thisOrg = dc.Orgs.First<Org>(o => o.Id == thisIssue.OrgId);
            //Vote thisVote = new Vote();
            var VC = dc.VoteCounts.Single(v => v.IssueId == thisIssue.Id && v.OrgId == thisOrg.Id);

            var userId = User.Identity.GetUserId();
            // Increment issue's vote counter, initialize our new vote for the issue/user, POST both to server; otherwise, log an error
            var user = dc.AspNetUsers.Single(u => u.Id == userId);
            // Increment issue's vote counter, initialize our new vote for the issue/user, POST both to server; otherwise, log an error

            try
            {
                thisIssue.votes++;
                thisIssue.lastVoted = DateTime.UtcNow;

                var newVC = CountXML<VoteCountCol>.XMLtoDATA(VC.count);
                newVC.Update();
                VC.count = CountXML<VoteCountCol>.DATAtoXML(newVC);

                var newUV = CountXML<UserVoteCol>.XMLtoDATA(user.votes);
                newUV.Add_Vote(thisIssue.Id, thisOrg.Id);
                user.votes = CountXML<UserVoteCol>.DATAtoXML(newUV);

                dc.SubmitChanges();
            }
            catch ( Exception e )
            {
                ErrorHandler.Log_Error(VC, e);
                ErrorHandler.Log_Error(user, e);
                //Console.WriteLine( "Could not vote on issue %s:\n%s", thisIssue.Id, e.Message );
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
            Issue thisIssue = dc.Issues.Single(i => i.Id == issueID);
            Org thisOrg = dc.Orgs.Single(o => o.Id == thisIssue.OrgId);
            var userId = User.Identity.GetUserId();

            // Decrement vote counter for issue, delete a vote from the votecount entry for the issue, POST changes to server; otherwise, log an error
            try {
                thisIssue.votes--;
                thisIssue.lastVoted = DateTime.UtcNow;

                var VC = dc.VoteCounts.Single(v => v.IssueId == thisIssue.Id && v.OrgId == thisOrg.Id);
                var vcol = CountXML<VoteCountCol>.XMLtoDATA(VC.count);
                vcol.Remove_Vote();
                VC.count = CountXML<VoteCountCol>.DATAtoXML(vcol);

                var user = dc.AspNetUsers.Single(u => u.Id == userId);
                var userVotes = CountXML<UserVoteCol>.XMLtoDATA(user.votes);
                userVotes.Delete_Vote(thisIssue.Id, thisOrg.Id);

                user.votes = CountXML<UserVoteCol>.DATAtoXML(userVotes);

                dc.SubmitChanges();

            }
            catch ( Exception e )
            {
                ErrorHandler.Log_Error(thisIssue, e);
                ErrorHandler.Log_Error(userId, e);
            }

            return new EmptyResult();
        }

        private void Notification_Check(string text, string user)
        {
            string to_user = null;
            string to_org = null;
            var text_words = text.Split(' ');
            foreach (string t in text_words)
            {
                if (t[0] == '@')
                {
                    to_user = t.Remove(0, 1);
                    
                }

            }

            if (to_user != null || to_org != null)
            {
                var to_user_id = from users in dc.AspNetUsers
                                 where users.UserName == to_user
                                 select users.Id;

                var to_org_id = from org in dc.Orgs
                                where org.orgURL == to_org
                                select org.Id;

                Notification note = new Notification();
                try
                {
                    note.From_UserId = user;
                    note.To_UserId = to_user_id.Cast<string>().First();
                    //note.To_OrgId = Convert.ToInt32(to_org_id.ToString());
                    note.createTime = DateTime.UtcNow;
                    dc.Notifications.InsertOnSubmit(note);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    ErrorHandler.Log_Error(note, e);
                    //Console.WriteLine("Could not unvote on issue %s:\n%s", note.Id, e.Message);
                }
            }
        }

        /* 
        ==================== 
        TimeSince
  
            Returns a string telling the time since the date posted of any issue or otherwise (comment, registration, etc.) 
        ==================== 
        */

        public static string TimeSince(DateTime datePosted) {
            DateTime now = DateTime.Now;
            TimeSpan since = now - datePosted;

            if ( since >= TimeSpan.FromDays( 365.0 ) ) {
                int years = since.Days / 365;
                return ( years > 1 ) ? years.ToString() + " years ago" : "1 year ago";
            } else if ( since >= TimeSpan.FromDays( 30.0 ) ) {
                int months = since.Days / 30;
                return ( months > 1 ) ? months.ToString() + " months ago" : "1 month ago";
            } else if ( since >= TimeSpan.FromDays( 1.0 ) ) {
                return ( since.Days > 1 ) ? since.Days.ToString() + " days ago" : "1 day ago";
            } else if ( since >= TimeSpan.FromHours( 1.0 ) ) {
                return ( since.Hours > 1 ) ? since.Hours.ToString() + " hours ago" : "1 hour ago";
            } else if ( since >= TimeSpan.FromMinutes( 1.0 ) ) {
                return ( since.Minutes > 1 ) ? since.Minutes.ToString() + " minutes ago" : "1 minute ago";
            } else {
                return "Less than a minute ago";
            }

        }

        /*public static Tuple<VoteState, int> VoteButtonModel(HttpContext currentContext, int issueID) {
            if ( !currentContext.Request.IsAuthenticated ) {
                return new Tuple<VoteState, int>( VoteState.NotLoggedIn, 0 );
            }
            AspNetUser user = dc.AspNetUsers.SingleOrDefault( u => u.Id == userID );
            UserVoteCol userVote = CountXML<UserVoteCol>.XMLtoDATA( user.votes );

            currentContext.User.Identity.GetUserId()
        }*/

        // End class IssueController
    }
    // End namespace Sigil.Controllers
}
