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
                IQueryable<Subscription> userSubs = from Subs in dc.Subscriptions
                                                    where Subs.UserId == userID
                                                    select Subs;

                IQueryable<Issue> userIssues = from issue in dc.Issues
                                               where userSubs.Any(s => s.OrgId == issue.OrgId)
                                               orderby issue.votes descending
                                               select issue;
                //userIssues.OrderBy(i => i.votes).ThenBy(i => i.createTime);

                IQueryable<Vote> userVotes = from vote in dc.Votes
                                             where vote.UserID == userID
                                             select vote;

                Tuple<IQueryable<Issue>, IQueryable<Vote>> issuesANDvotes = new Tuple<IQueryable<Issue>, IQueryable<Vote>>(userIssues, userVotes);


                return View(issuesANDvotes);
            }
            else
            {
                //IQueryable<Issue> allIssues = from iss in dc.Issues
                //                              select iss;
                //allIssues.OrderBy(i => i.lastVoted).ThenByDescending(i => i.votes);

                //Tuple<IQueryable<Issue>, IQueryable<Vote>> issuesANDvotes = new Tuple<IQueryable<Issue>, IQueryable<Vote>>(allIssues, null);
                return RedirectToAction("LandingPage");
            }
            
        }

        public ActionResult LandingPage()
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


        public JsonResult Search(string term)
        {
            List<string> issue_list;
            if (string.IsNullOrEmpty(term))
            {
                issue_list = dc.Issues.Select(i => i.title).ToList();
            }
            else
            {
               var issue_qu = from iss in dc.Issues
                             where iss.title.StartsWith(term)
                             select iss;
                issue_list = issue_qu.Select(i => i.title).ToList();
            }

            return Json(issue_list, JsonRequestBehavior.AllowGet);
        }
    }
}