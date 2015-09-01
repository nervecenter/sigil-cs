using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;



namespace Sigil.Controllers {

    public class HomeController : Controller {

        private SigilDBDataContext dc;

        //Comparison Object for sorting
        private static Comparison<Issue> Rank = new Comparison<Issue>(Issue_Compare);

        public HomeController()
        {
            dc = new SigilDBDataContext();
        }

        
        public ActionResult Index() {
            var userID = User.Identity.GetUserId();

            if (userID != null)
            {

                IQueryable<Subscription> userSubs = from Subs in dc.Subscriptions
                                                    where Subs.UserId == userID
                                                    select Subs;

                var userIssues = Get_User_Issues(userID, userSubs).ToList();

                userIssues.Sort(Rank);
                
                IQueryable<Vote> userVotes = from vote in dc.Votes
                                             where vote.UserID == userID
                                             select vote;

                Tuple<List<Issue>, List<Vote>> issuesANDvotes = new Tuple<List<Issue>, List<Vote>>(userIssues, userVotes.ToList());

                return View(issuesANDvotes);
            }
            else 
            {
                return LandingPage();
                
            }
        }

        public ActionResult LandingPage()
        {
            var trending_topics = Get_Trending_Issues();


            return View("LandingPage", trending_topics);
        }


        public ActionResult Legal()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //==================================================================== Helper Functions =========================================================================================

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
        private List<Issue> Get_Trending_Issues()
        {
            var pretrending = from iss in dc.Issues
                              where iss.TopicId != 0
                              select iss;

            List < Tuple<Issue,double> > issue_ranks = new List<Tuple<Issue,double>>();
            foreach(Issue i in pretrending)
            {
                
                issue_ranks.Add(new Tuple<Issue, double>(i, Calculate_Rank(i)));
            }

            issue_ranks.Sort(delegate (Tuple<Issue, double> x, Tuple<Issue, double> y)
            {
                return x.Item2.CompareTo(y.Item2);
            });

            var trending = issue_ranks.OrderByDescending(i => i.Item2).Select(i => i.Item1).Take(3).ToList();

            return trending;
        }


        static DateTime test_date = new DateTime(2015, 7, 17, 13, 33, 57);
        /// <summary>
        /// Calculates and returns the passed in issues rank. Based off the Reddit trending formula.
        /// </summary>
        /// <param name="issue">Issue you want the rank of.</param>
        /// <returns></returns>
        private static double Calculate_Rank(Issue issue)
        {
            //Date of oldest submission as of right now ---> 7 / 17 / 2015 13:33:57
            TimeSpan secs = issue.createTime - test_date;

            return Math.Round(Math.Log(issue.votes + 1) - Math.Log(Convert.ToDouble(issue.votes + issue.viewCount) + 2) + secs.TotalSeconds / 19543, 7);

        }

        /// <summary>
        /// Comparison function called by the comparison object for sorting lists
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
                return -1;
            else
                return 1;
        }



    }
}