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

        public JsonResult SearchOrgs(string term)
        {
            List<string> search_list = new List<string>();
            if (!string.IsNullOrEmpty(term))
            {
                search_list.AddRange(search_orgs(term));
            }
            return Json(search_list, JsonRequestBehavior.AllowGet);
        }

        /* 
        =============================================================================================================================================
        Helper functions for various searches
        =============================================================================================================================================
        */

        private List<string> search_users(string term)
        {

            var qu = from user in dc.AspNetUsers
                          where user.UserName.StartsWith(term)
                          select user;

            return qu.Select(u => u.UserName).ToList();

        }

        private List<string> search_orgs(string term)
        {

            var qu = from org in dc.Orgs
                          where org.orgName.StartsWith(term)
                          select org;

            return qu.Select(o => o.orgName).ToList();

        }

        private List<string> search_issues(string term)
        {

            var qu = from iss in dc.Issues
                          where iss.title.StartsWith(term)
                          select iss;

            return qu.Select(i => i.title).ToList();

        }
    }
}