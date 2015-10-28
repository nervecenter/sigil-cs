using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
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
                
                CommentController.Create_New_Comment(Request, thisIssue, userID);
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
                    ErrorHandler.Log_Error(vc, e, dc);
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
                    ErrorHandler.Log_Error(thisIssue, e, dc);

                }
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
                ErrorHandler.Log_Error(issue, e, dc);

            }

        }

        /* 
        ==================== 
        IssueData
  
            Data page for an issue, showing views/votes this week/month 
        ==================== 
        */

        public JsonResult DefaultData(string orgURL, int issueId)
        {
            Org thisOrg = dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);

            var issueViews = CountXML<ViewCountCol>.XMLtoDATA(dc.ViewCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);

            var View_Data = DataVisualization.Data_to_Google_Graph_Format(issueViews, DateTime.Now.AddDays(-7), DateTime.Now);

            return Json(View_Data.Select(d => new { viewDate = d.Item1, viewCount = d.Item2 }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CustomData(string orgURL, int issueId ,string dataType, string start, string stop)
        {
            Org thisOrg = dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);

            DateTime startDate = DateTimeConversion.FromJSms(start);

            DateTime stopDate = DateTimeConversion.FromJSms(stop);

            List<Tuple<long, int>> view_data = new List<Tuple<long, int>>();

            switch (dataType)
            {
                case "Views":
                    {
                        var data = CountXML<ViewCountCol>.XMLtoDATA(dc.ViewCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Votes":
                    {
                        var data = CountXML<VoteCountCol>.XMLtoDATA(dc.VoteCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Comments":
                    {
                        var data = CountXML<CommentCountCol>.XMLtoDATA(dc.CommentCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);
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

        [Authorize]
        public ActionResult IssueData(string orghandle, long issueID)
        {
            // Get the issue
            Issue thisIssue = dc.Issues.SingleOrDefault(i => i.Id == issueID);

            // Get the org
            Org thisOrg = thisIssue.Org;
            /*
             * VIEWBAG
             */

            // Add the issue and org to the ViewBag
            ViewBag.thisIssue = thisIssue;
            ViewBag.thisOrg = thisOrg;


            return View();
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
                ErrorHandler.Log_Error(newVote, e, dc);
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
                ErrorHandler.Log_Error(newissue, e, dc);
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
                ErrorHandler.Log_Error(VC, e, dc);
                ErrorHandler.Log_Error(user, e, dc);
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
                ErrorHandler.Log_Error(thisIssue, e, dc);
                ErrorHandler.Log_Error(userId, e, dc);
            }

            return new EmptyResult();
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
