using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Sigil.Controllers
{
    public class SearchController : Controller
    {
        private SigilDBDataContext dc;

        public SearchController()
        {
            dc = new SigilDBDataContext();
        }
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SearchDB(string term)
        {
            List<string> search_list = new List<string>();
            if(!string.IsNullOrEmpty(term))
            {
                search_list.AddRange(search_users(term));
                search_list.AddRange(search_orgs(term));
                search_list.AddRange(search_issues(term));
            }

            return Json(search_list, JsonRequestBehavior.AllowGet);
        }

        private List<string> search_users(string term)
        {

            var user_qu = from user in dc.AspNetUsers
                          where user.UserName.StartsWith(term)
                          select user;

            return user_qu.Select(u => u.UserName).ToList();

        }

        private List<string> search_orgs(string term)
        {

            var user_qu = from org in dc.Orgs
                          where org.orgName.StartsWith(term)
                          select org;

            return user_qu.Select(o => o.orgName).ToList();

        }

        private List<string> search_issues(string term)
        {

            var user_qu = from iss in dc.Issues
                          where iss.title.StartsWith(term)
                          select iss;

            return user_qu.Select(i => i.title).ToList();

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