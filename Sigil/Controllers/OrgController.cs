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
            List<UserVote> userVotes;

            if (userId != null)
            {
                // Get the user's votes on this org
                userVotes = CountXML<UserVoteCol>.XMLtoDATA(dc.AspNetUsers.Single(u => u.Id == userId).votes).Get_Votes();
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

            ViewBag.userSub = dc.Subscriptions.SingleOrDefault(s => s.UserId == userId && s.OrgId == thisOrg.Id);

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
            var orgIssueViews = (from vc in dc.ViewCounts
                                 where vc.Issue.OrgId == thisOrg.Id
                                 select CountXML<ViewCountCol>.XMLtoDATA(vc.count));

            // For each day in the week, get that day's votes on all issues in the org, group them into a week of integers of votes
            var orgIssueVotes = (from vote in dc.VoteCounts
                                 where vote.Issue.OrgId == thisOrg.Id
                                 select CountXML<VoteCountCol>.XMLtoDATA(vote.count));

            var weekChart = DataVisualization.Create_Highchart(orgIssueViews, orgIssueVotes, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow, "weekChart", "Traffic on " + thisOrg.orgName + " This Month");


            // Add week chart to our list, get the total counts for views and votes over week, add them and turnover rate to ViewBag
            listOfCharts.Add(weekChart);
            var weekSums = DataVisualization.Get_Sum(orgIssueViews, orgIssueVotes, DateTime.UtcNow.AddDays(-6), DateTime.UtcNow);

            ViewBag.weekViewCount = weekSums.Item1;
            ViewBag.weekVoteCount = weekSums.Item2;
            ViewBag.weekRatio = ((double)weekSums.Item2 / (double)weekSums.Item1) * 100.0f;

            /*
             *  MONTHLY Traffic Data
             */


            // TODO: Possible solution: For each day up to current, add datestring to array, and add votecount to array; if no entry, add 0

            // Create a Highchart with X-axis for days of the month, and Y-axis series logging views and votes
            Highcharts monthChart = DataVisualization.Create_Highchart(orgIssueViews, orgIssueVotes, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow, "monthChart", "Traffic on " + thisOrg.orgURL + " This Month");

            // Add month chart to our list, get the total counts for views and votes over month, add them and turnover rate to ViewBag
            listOfCharts.Add(monthChart);
            var monthSums = DataVisualization.Get_Sum(orgIssueViews, orgIssueVotes, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);

            ViewBag.monthViewCount = monthSums.Item1;
            ViewBag.monthVoteCount = monthSums.Item2;
            ViewBag.monthRatio = ((double)monthSums.Item2 / (double)monthSums.Item1) * 100.0f;


            /*
             * VIEWBAG
             */

            // Add the org to the ViewBag
            ViewBag.thisOrg = thisOrg;

            // Pass our model list of charts as the model of the view
            return View(listOfCharts);
        }
        //
        //// GET: /orgregister
        //[AllowAnonymous]
        //public ActionResult OrgRegister()
        //{
        //    return View();
        //}

        ////
        //// POST: /orgregister
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> OrgRegister(OrgRegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}


    }
}