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

                Tuple<List<Issue>, List<Vote>> issuesANDvotes = new Tuple<List<Issue>, List<Vote>>(userIssues.ToList(), userVotes.ToList());

                ViewBag.userSubs = userSubs.ToList();

                return View( issuesANDvotes );
            } 
            else 
            {
                //return View(LandingPage());
                return View( "LandingPage" );
            }
        }

        public ActionResult LandingPage()
        {
            //var trending_topics = Get_Trending_Topics();

            //var trending_issues = Get_Trending_Issues(trending_topics);

            return View();
        }

        //private IQueryable<Issue> Get_Trending_Issues(IQueryable<Topic> trending_topics)
        //{
        //    var pre_issue = dc.Issues.OrderBy(i => i.votes).ThenBy(i => i.viewCount).ThenByDescending(i => i.createTime);

            
        //    foreach (Topic t in trending_topics)
        //    {

        //    }

                              
        //    return trend_issue;
                              
                               
                                
        //}

        //private IQueryable<Issue> Get_Trending_Topics()
        //{
        //    //var trending = dc.Topics.OrderBy(t => t.lastAdded).ThenBy(t => t.views).Take(3);
        //    var pretrending = dc.Issues.Where(i => i.TopicId != null).OrderBy(i => i.lastVoted).ThenBy(i => i.votes);

        //    var trending = pretrending.GroupBy(i => i.TopicId).Select(grp => grp.First()).Take(3);

        //    return trending;
        //}


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