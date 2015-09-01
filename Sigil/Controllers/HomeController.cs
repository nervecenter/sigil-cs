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

        public HomeController()
        {
            dc = new SigilDBDataContext();
        }


        public ActionResult Index() {
            var userID = User.Identity.GetUserId();

            if (userID != null)
            {
                // User is logged in; load the normal frontpage
                //AspNetUser user = dc.AspNetUsers.Single<AspNetUser>( u => u.Id == userID );

                IQueryable<Subscription> userSubs = from Subs in dc.Subscriptions
                                                    where Subs.UserId == userID
                                                    select Subs;

                IQueryable<Issue> userIssues = from issue in dc.Issues
                                               where userSubs.Any(s => s.OrgId == issue.OrgId)
                                               orderby issue.votes descending
                                               select issue;

              
                IQueryable<Vote> userVotes = from vote in dc.Votes
                                             where vote.UserID == userID
                                             select vote;

                Tuple<IQueryable<Issue>, IQueryable<Vote>> issuesANDvotes = new Tuple<IQueryable<Issue>, IQueryable<Vote>>(userIssues, userVotes);

                return View( issuesANDvotes );
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



        private List<Issue> Get_Trending_Issues()
        {
            var pretrending = from iss in dc.Issues
                              where iss.TopicId != null
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

        private double Calculate_Rank(Issue issue)
        {
            //Date of oldest submission as of right now ---> 7 / 17 / 2015 13:33:57
            TimeSpan secs = issue.createTime - test_date;

            return Math.Round(Math.Log(issue.votes + 1) - Math.Log(Convert.ToDouble(issue.votes + issue.viewCount) + 2) + secs.TotalSeconds / 19543, 7);

        }


        public ActionResult Legal()
        {
            return View();
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}