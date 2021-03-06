using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using Sigil.ViewModels;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Data.SqlTypes;
using Sigil.Services;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Sigil.Controllers
{
    public class IssueController : Controller
    {

        private readonly IOrgService orgService;
        private readonly IIssueService issueService;
        private readonly ICommentService commentService;
        private readonly IUserService userService;
        private readonly ICountService countDataService;
        private readonly IErrorService errorService;
        private readonly IProductService productService;
        private Regex textFilter;

        public IssueController(IOrgService orgS, IIssueService issS, ICommentService comS, IUserService userS, ICountService countS, IErrorService errS, IProductService prodS)
        {
            orgService = orgS;
            issueService = issS;
            commentService = comS;
            userService = userS;
            countDataService = countS;
            errorService = errS;
            productService = prodS;

            textFilter = new Regex("[^0-9a-zA-Z . @]");
        }

        public ActionResult IssuePage( string orgURL, string productURL ,int issueID )
        {
            // Grab the issue's org
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.SingleOrDefault<Org>(o => o.orgURL == orgURL);

            Product thisProduct = productService.GetProduct(productURL);
            // Grab the issue for the page
            Issue thisIssue = issueService.GetIssue(issueID);
            //Issue thisIssue = (from issue in dc.Issues
            //                   where issue.Id == issueID
            //                   select issue).SingleOrDefault();

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
            UserViewModel userVM = default(UserViewModel);
            if (userID == null)
            {
                userVM = new UserViewModel().emptyUser();
            }
            else
            {
                userVM = userService.GetUserViewModel(userID);
            }

            // If the page is in POST, we're posting a comment; get form data and POST it
            if (Request.HttpMethod == "POST")
            {
                commentService.Comment_POST_Handler(Request, thisIssue, userID);
            }

            // Get the user's vote on this issues if it exists
            
            if (userID != null && userVM.User.OrgId == thisOrg.Id)
                userVM.isOrgAdmin = true;
            else
                userVM.isOrgAdmin = false;

            var issueComments = commentService.GetIssueComments(issueID);   

            var official = commentService.GetIssuesOfficialResponses(issueID);

            IssuePageViewModel viewModel = new IssuePageViewModel();

           
            viewModel.IssuePanelVM = new IssuePanelPartialVM() { issue = thisIssue, UserVoted = userVM.UserVotes.Check_Vote(thisIssue.Id), InPanel = false };

            viewModel.UserVM = userVM;

            viewModel.IssueOrg = thisOrg;

            viewModel.IssueProduct = thisProduct;

            //viewModel.sideBar = new SideBarVM() { thisOrg = thisOrg, Subscriptions = userVM.UserSubscriptions };
            viewModel.sideBar = new SideBarVM() { thisOrg = thisOrg };

            viewModel.OfficialResponses = official;
            
            viewModel.IssueComments = issueComments.Select(c => new Tuple<Comment, bool>(c, userVM.UserVotes.Check_Vote(issueID, c.Id)));

            // Pass the org and issue as the model to the view
            return View(viewModel);
        }

        /// <summary>
        /// Increments the issues ViewCount table entry or if not created yet, makes a new viewcount entry for the issue.
        /// </summary>
        /// <param name="thisOrg">Org of the passed in issue</param>
        /// <param name="thisIssue">Issue object from the table</param>
        private void ViewCount_Routine(Org thisOrg, Issue thisIssue)
        {
            ViewCountCol vc = countDataService.GetIssueViewCountCol(thisOrg.Id, thisIssue.Id);//dc.ViewCounts.FirstOrDefault<ViewCount>(v => v.IssueId == thisIssue.Id);//&& 

            vc.Update();
            countDataService.SaveCountChanges(vc, CountDataType.View,thisOrg.Id, thisIssue.Id);

        }

        /// <summary>
        /// Updates the total view count of the passed in issue.
        /// </summary>
        /// <param name="issue"></param>
        private void Update_View_Count(Issue issue)
        {

            issue.viewCount++;
            issueService.UpdateIssue(issue);
            issueService.SaveChanges();

        }

        public ActionResult Edit_Issue(int issueId)
        {
            throw new NotImplementedException();
        }

        public ActionResult Edit_Comment(int commentId)
        {
            throw new NotImplementedException();
        }


        public ActionResult Delete_Issue(int issueId)
        {
            Issue delete_me = issueService.GetIssue(issueId);
            delete_me.UserId = userService.GetUserByDisplayName("Deleted").Id;
            delete_me.title = "Deleted";
            delete_me.text = "Deleted";
            delete_me.editTime = DateTime.UtcNow;
            issueService.UpdateIssue(delete_me);
            issueService.SaveChanges();
            return RedirectToAction("IssuePage", "Issue", new { orgURL = delete_me.Product.Org.orgURL, productURL = delete_me.Product.ProductURL, issueID = issueId });
        }

        public ActionResult Delete_Issue_Comment(int commentId)
        {
            Comment delete_me = commentService.GetComment(commentId);
            delete_me.UserId = userService.GetUserByDisplayName("Deleted").Id;
            delete_me.text = "Deleted";
            delete_me.editTime = DateTime.UtcNow;
            commentService.UpdateComment(delete_me);
            //Issue commentIssue = issueService.GetIssue(delete_me.IssueId);
            //commentService.DeleteComment(delete_me);
            return RedirectToAction("IssuePage", "Issue", new { orgURL = delete_me.Issue.Product.Org.orgURL, productURL = delete_me.Issue.Product.ProductURL, issueID = delete_me.IssueId });
        }

        public JsonResult DefaultData(string orgURL, int issueId)
        {
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);

            var issueViews = countDataService.GetIssueViewCountCol(thisOrg.Id, issueId);//CountXML<ViewCountCol>.XMLtoDATA(dc.ViewCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);

            var View_Data = DataVisualization.Data_to_Google_Graph_Format(issueViews, DateTime.Now.AddDays(-7), DateTime.Now);

            return Json(View_Data.Select(d => new { viewDate = d.Item1, viewCount = d.Item2 }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CustomData(string orgURL, int issueId ,string dataType, string start, string stop)
        {
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);

            DateTime startDate = DateTimeConversion.FromJSms(start);

            DateTime stopDate = DateTimeConversion.FromJSms(stop);

            List<Tuple<long, int>> view_data = new List<Tuple<long, int>>();

            switch (dataType)
            {
                case "Views":
                    {
                        var data = countDataService.GetIssueViewCountCol(thisOrg.Id, issueId);//CountXML<ViewCountCol>.XMLtoDATA(dc.ViewCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Votes":
                    {
                        var data = countDataService.GetIssueVoteCountCol(thisOrg.Id, issueId);//CountXML<VoteCountCol>.XMLtoDATA(dc.VoteCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Comments":
                    {
                        var data = countDataService.GetIssueCommentCountCol(thisOrg.Id, issueId);//CountXML<CommentCountCol>.XMLtoDATA(dc.CommentCounts.Single(vc => vc.IssueId == issueId && vc.OrgId == thisOrg.Id).count);
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
        public ActionResult IssueData(string orgURL, string productURL,int issueID)
        {
            return View();
        }

        [Authorize]
        public ActionResult AddIssue_Post()
        {
            // Get the org for the issue we're adding

            // Get the user
            var userId = User.Identity.GetUserId();
            AddIssueVM vm = (AddIssueVM)TempData["vm"];

            //pass in creators userid, the org and possible Product of the issue
            int issueId = Create_New_Issue(userId, vm.org, vm.product, vm);

            if (issueId == 0)
            {
                //this is where we need to redirect to a page if the issue posting failed in the previous function call
            }

            New_Issue_Data_Routine(userId, vm.org.Id, issueId);

            return Redirect("~/" + vm.org.orgURL+ "/"+ vm.product.ProductURL + "/" + issueId);

        }

        private void New_Issue_Data_Routine(string userId, int orgId, int issueId)
        {
            //Create Vote count table entry
            var user = userService.GetUser(userId);

            countDataService.CreateIssueCountData(user.Id, orgId, issueId);
            countDataService.SaveIssueCountData();

            //update the users votes xml ds because every user votes on the issue they make
            userService.AddUserVote(user, issueId);
            userService.UpdateUser(user);
            userService.SaveUserVotes();
        }

        private int Create_New_Issue(string userId, Org org, Product product, AddIssueVM vm)
        {
            // Create a new issue
            Issue newissue = new Issue();

            newissue.UserId = userId;
            newissue.createTime = DateTime.UtcNow;
            newissue.editTime = DateTime.UtcNow;
            newissue.lastVoted = DateTime.UtcNow;
            newissue.votes = 1;
            newissue.viewCount = 1;
            newissue.title = vm.title;
            newissue.text = textFilter.Replace(vm.text, "");
            newissue.ProductId = product == default(Product) ? 0 : product.Id;
            newissue.ProductId = product.Id;
            
            // Try to submit the issue and go to the issue page; otherwise, write an error
            try
            {
                //dc.Issues.InsertOnSubmit(newissue);
                //dc.SubmitChanges();
                issueService.CreateIssue(newissue);
                issueService.SaveChanges();
                
                return newissue.Id;//dc.Issues.Last(i => i.UserId == userId && i.OrgId == org.Id).Id;
            }
            catch
            {
                //ErrorHandler.Log_Error(newissue, e, dc);
                return 0; //need to return a 404 or just rediret to another page that include an error message for the user
            }
        }

        /// <summary>
        /// Issue Vote up
        /// </summary>
        /// <param name="IssueId">Id of the issue that is being upvoted</param>
        /// <returns>An empty action result ment to be handeled and called useing javascript and jquery.</returns>
        [Authorize]
        public ActionResult VoteUp(int IssueId) {

            var userId = User.Identity.GetUserId();

            var issue = issueService.GetIssue(IssueId);
            var user = userService.GetUser(userId);

            userService.AddUserVote(user, IssueId);

            issue.votes++;
            issue.lastVoted = DateTime.UtcNow;
            issueService.UpdateIssue(issue);
            issueService.SaveChanges();
            countDataService.UpdateIssueVoteCountData(issue);
            countDataService.SaveIssueCountData();
            userService.UpdateUser(user);
            userService.SaveUserVotes();

            return new EmptyResult();
        }

        /// <summary>
        /// Comment VoteUp
        /// </summary>
        /// <param name="IssueId">IssueId of that owns the comment.</param>
        /// <param name="CommentId">Id of the comment being upvoted.</param>
        /// <returns>An empty action result. Ment to be called using js and jquery.</returns>
        [Authorize]
        public ActionResult VoteUpComment(int IssueId, int CommentId)
        {

            var userId = User.Identity.GetUserId();

            var comment = commentService.GetComment(CommentId);
            var user = userService.GetUser(userId);

            userService.AddUserVote(user, IssueId, CommentId);

            comment.votes++;
            comment.lastVoted = DateTime.UtcNow;
            commentService.UpdateComment(comment);

             
            userService.UpdateUser(user);

            return new EmptyResult();
        }


        /// <summary>
        /// Issue Remove Vote
        /// </summary>
        /// <param name="IssueId">Id of the issue that is having a vote removed.</param>
        /// <returns>An empty action result. Ment to be called using js and jquery.</returns>
        [Authorize]
        public ActionResult UnVoteUp(int IssueId) {

            var userId = User.Identity.GetUserId();
            var user = userService.GetUser(userId);
            var issue = issueService.GetIssue(IssueId);
           

            issue.votes--;
            issue.lastVoted = DateTime.UtcNow;
            issueService.UpdateIssue(issue);
            issueService.SaveChanges();
            countDataService.UpdateIssueVoteCountData(issue, false);
            countDataService.SaveIssueCountData();
            userService.RemoveUserVote(user, IssueId);
            userService.UpdateUser(user);
            userService.SaveUserVotes();
            return new EmptyResult();
        }

        /// <summary>
        /// Issue Remove Vote
        /// </summary>
        /// <param name="IssueId">Id of the issue that is having a vote removed.</param>
        /// <returns>An empty action result. Ment to be called using js and jquery.</returns>
        [Authorize]
        public ActionResult UnVoteUpComment(int IssueId, int CommentId)
        {

            var userId = User.Identity.GetUserId();
            var user = userService.GetUser(userId);

            var comment = commentService.GetComment(CommentId);

            comment.votes--;
            comment.lastVoted = DateTime.UtcNow;
            commentService.UpdateComment(comment);

            userService.RemoveUserVote(user, IssueId, CommentId);
            userService.UpdateUser(user);


            return new EmptyResult();
        }

        /* 
        ==================== 
        TimeSince
  
            Returns a string telling the time since the date posted of any issue or otherwise (comment, registration, etc.) 
        ==================== 
        */

        //!!!!!!!!! need to make this a JS function!!!

        //public static string TimeSince(DateTime datePosted) {
        //    DateTime now = DateTime.Now;
        //    TimeSpan since = now - datePosted;

        //    if ( since >= TimeSpan.FromDays( 365.0 ) ) {
        //        int years = since.Days / 365;
        //        return ( years > 1 ) ? years.ToString() + " years ago" : "1 year ago";
        //    } else if ( since >= TimeSpan.FromDays( 30.0 ) ) {
        //        int months = since.Days / 30;
        //        return ( months > 1 ) ? months.ToString() + " months ago" : "1 month ago";
        //    } else if ( since >= TimeSpan.FromDays( 1.0 ) ) {
        //        return ( since.Days > 1 ) ? since.Days.ToString() + " days ago" : "1 day ago";
        //    } else if ( since >= TimeSpan.FromHours( 1.0 ) ) {
        //        return ( since.Hours > 1 ) ? since.Hours.ToString() + " hours ago" : "1 hour ago";
        //    } else if ( since >= TimeSpan.FromMinutes( 1.0 ) ) {
        //        return ( since.Minutes > 1 ) ? since.Minutes.ToString() + " minutes ago" : "1 minute ago";
        //    } else {
        //        return "Less than a minute ago";
        //    }

        //}

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
