﻿using System;
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
                AspNetUser user = dc.AspNetUsers.Single<AspNetUser>( u => u.Id == userID );

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

                return View( issuesANDvotes );
            } 
            else 
            {
                return View( "LandingPage" );
            }
        }

        public ActionResult LandingPage()
        {
            /*var trending_topics = Get_Trending_Topics();

            var trending_issues = Get_Trending_Issues(trending_topics);*/

            return View();
        }

        private object Get_Trending_Issues(IQueryable<Topic> trending_topics)
        {
            throw new NotImplementedException();
        }

        /*private IQueryable<Topic> Get_Trending_Topics()
        {


        }*/


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