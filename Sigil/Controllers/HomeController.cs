using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Sigil.Controllers {

    public class HomeController : Controller {

        private SigilDBDataContext dc;

        //Comparison Object for sorting
        private static Comparison<Issue> Rank = new Comparison<Issue>(Issue_Compare);

        public HomeController()
        {
            dc = new SigilDBDataContext();
        }

        
        public ActionResult Index(int? page) {
            var userID = User.Identity.GetUserId();

            if (userID != null)
            {
                //Get the users subscriptions
                IQueryable<Subscription> userSubs = dc.Subscriptions.Where(s => s.UserId == userID);

                //get all the users issues based on their subscriptions
                var userIssues = Get_User_Issues(userID, userSubs);

                //sort the users issues by issue rank

                userIssues.OrderBy(i => Rank);

                //gather all the votes the user made 
                var user = dc.AspNetUsers.Single(u => u.Id == userID);

                //this needs to be created in the user registration !!!!!!
                if (user.votes == null)
                    user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol());

                dc.SubmitChanges();
                var userVotes = CountXML<UserVoteCol>.XMLtoDATA(user.votes);

              

                int num_results_per_page = 6;
                int pageNumber = (page ?? 1);
                Tuple<PagedList.IPagedList<Sigil.Models.Issue>, UserVoteCol> issuesANDvotes = new Tuple<PagedList.IPagedList<Sigil.Models.Issue>, UserVoteCol>(userIssues.ToPagedList(pageNumber, num_results_per_page), userVotes);
                return View(issuesANDvotes);
            }
            else 
            {
                return LandingPage();
                
            }
        }

        public ActionResult LandingPage()
        {
            var trending_topics = Get_Trending_Issues_With_Topics();

            return View("LandingPage", trending_topics);
        }

        public ActionResult FeaturesPage() {
            return View( "FeaturesPage" );
        }

        public ActionResult Legal()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }




        //==================================================================== Helper Functions =============================================================

        /// <summary>
        /// Returns all issues associated with the users passed in subscriptions.
        /// </summary>
        /// <param name="userID">Users AspNetUser.Id</param>
        /// <param name="userSubs">Query of all subscriptions assocaited with the user.</param>
        /// <returns></returns>
        private IQueryable<Issue> Get_User_Issues(string userID, IQueryable<Subscription> userSubs)
        {
            return dc.Issues.Where(i => userSubs.Any(s => s.OrgId == i.OrgId || (s.TopicId != 0 && s.TopicId == i.TopicId) || i.UserId == userID || (s.CatId != 0 && s.CatId == i.CatId)));
        }


        /// <summary>
        /// Gets the Top 3 trending issues site wide. FOR THE LANDING PAGE ONLY
        /// </summary>
        /// <returns></returns>
        private List<IGrouping<Org,Issue>> Get_Trending_Issues_With_Topics()
        {
            var pretrending = (from iss in dc.Issues
                              select iss).ToList();

            pretrending.Sort(Rank);

            var trending = pretrending.GroupBy(i => i.Org).ToList();

            return trending;
        }




        static DateTime test_date = new DateTime(2015, 7, 17, 13, 33, 57); //<------------ This will be changed once we go live.
        /// <summary>
        /// Calculates and returns the passed in issues rank. Based off the Reddit trending formula. 
        /// </summary>
        /// <param name="issue">Issue you want the rank of.</param>
        /// <returns>A double value with 7 decimal places</returns>
        private static double Calculate_Rank(Issue issue)
        {
            //Date of oldest submission as of right now ---> 7 / 17 / 2015 13:33:57
            TimeSpan secs_alive = issue.createTime - test_date;
            TimeSpan secs_voted = issue.lastVoted - issue.createTime;
            var r = Math.Round(Math.Log(issue.votes + 1) - Math.Log(Convert.ToDouble(issue.votes + issue.viewCount) + 2) + (secs_alive.TotalSeconds + secs_voted.TotalSeconds) / 19543, 7);
            return r;

        }




        /// <summary>
        /// Comparison function called by the comparison object for sorting lists. As of now sorts in decending order.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static int Issue_Compare(Issue x, Issue y)
        {
            var rank_x = Calculate_Rank(x);
            var rank_y = Calculate_Rank(y);

            if (rank_x == rank_y)
                return 0;
            else if (rank_x < rank_y)
                return 1;
            else
                return -1;
        }
    }
}