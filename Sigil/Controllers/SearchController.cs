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
        public ActionResult Index(string term)
        {

            var quUsers = dc.AspNetUsers.Where(u => u.DisplayName.StartsWith(term)).ToList();
            var quOrgs = dc.Orgs.Where(o => o.orgName.StartsWith(term)).ToList();
            var quIssues = dc.Issues.Where(i => i.title.StartsWith(term)).ToList();

            Tuple<List<AspNetUser>, List<Org>, List<Issue>> search_list = new Tuple<List<AspNetUser>, List<Org>, List<Issue>>(quUsers, quOrgs, quIssues);
            return View(search_list);

        }

        public JsonResult SearchDB(string term)
        {
            Dictionary<string, object> search_list = new Dictionary<string, object>();
            Dictionary<string, string> Final_search_list = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(term))
            {
                //search_list.AddRange(search_users(term));
                search_list["Org"] = search_orgs(term);
                
                search_list["Issue"] = search_issues(term);
            }

            foreach(string k in search_list.Keys)
            {
                if(k == "Org")
                {
                    List<Org> found_orgs = (List<Org>)search_list[k];
                    foreach(Org o in found_orgs)
                    {
                        Final_search_list[o.orgName] = "/" + o.orgName;
                    }
                }
                else if(k == "Issue")
                {
                    List<Issue> found_orgs = (List<Issue>)search_list[k];
                    foreach (Issue i in found_orgs)
                    {
                        Final_search_list[i.title] = "/" + i.Org.orgName + "/" + i.Id;
                    }
                }
            }

            return Json(Final_search_list.Select(s => new { label = s.Key, value = s.Value }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult SearchOrgs(string term)
        //{
        //    List<string> search_list = new List<string>();
        //    if (!string.IsNullOrEmpty(term) )
        //    {
        //        search_list.AddRange(search_orgs(term));
        //    }
        //    return Json(search_list, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SearchOrgs_Cats(string term)
        {
            List<string> search_list = new List<string>();
            if (!string.IsNullOrEmpty(term))
            {
                search_list.AddRange(search_orgs(term).Select(o => o.orgName));
                search_list.AddRange(search_orgs_and_cats(term));
            }

            return Json(search_list, JsonRequestBehavior.AllowGet);
        }



        /* 
        =============================================================================================================================================
        Helper functions for various searches
        =============================================================================================================================================
        */

        private List<string> search_orgs_and_cats(string term)
        {
            var qu = from org in dc.Orgs
                     from cat in dc.Categories
                     where org.orgName.StartsWith(term)
                     where cat.orgId == org.Id 
                     select (org.orgName + "-" + cat.catName);
            return qu.ToList();
        }

        private List<string> search_users(string term)
        {

            var qu = from user in dc.AspNetUsers
                          where user.DisplayName.StartsWith(term)
                          select user;

            return qu.Select(u => u.DisplayName).ToList();

        }

        private List<Org> search_orgs(string term)
        {

            var qu = from org in dc.Orgs
                          where org.orgName.StartsWith(term)
                          select org;

            return qu.Select(o => o).ToList();

        }

        private List<Issue> search_issues(string term)
        {

            var qu = from iss in dc.Issues
                          where iss.title.StartsWith(term)
                          select iss;

            return qu.Select(i => i).ToList();

        }
    }
}