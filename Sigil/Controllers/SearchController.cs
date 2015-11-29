using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Sigil.Services;

namespace Sigil.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductService productService;
        private readonly ISearchService searchService;
         
        public SearchController(IProductService prodS, ISearchService searchS)
        {
            productService = prodS;
            searchService = searchS;
        }

        // GET: Search
        public ActionResult Index()
        {
            string term = Request.Form["searchTerm"];

            var quUsers = searchService.MatchUsersByName(term).ToList();//dc.AspNetUsers.Where(u => u.DisplayName.StartsWith(term)).ToList();
            var quOrgs = searchService.MatchOrgsByName(term).ToList();//dc.Orgs.Where(o => o.orgName.StartsWith(term)).ToList();
            var quIssues = searchService.MatchIssuesByTitle(term).ToList();//dc.Issues.Where(i => i.title.StartsWith(term)).ToList();


            Tuple<List<ApplicationUser>, List<Org>, List<Issue>> search_list = new Tuple<List<ApplicationUser>, List<Org>, List<Issue>>(quUsers, quOrgs, quIssues);
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
                        Final_search_list[i.title] = "/" + i.Product.Org.orgName + "/" + i.Id;
                    }
                }
            }

            return Json(Final_search_list.Select(s => new { label = s.Key, value = s.Value }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult SearchOrgs_Cats(string term)
        {
            List<string> search_list = new List<string>();
            if (!string.IsNullOrEmpty(term))
            {
                //search_list.AddRange(search_orgs(term).Select(o => o.orgName));
                search_list.AddRange(search_orgs_and_cats(term));
            }

            return Json(search_list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchIssuesByOrg( int id ) 
        {
            IEnumerable<Issue> issues = searchService.MatchIssuesByOrg( id ).OrderByDescending( i => i.votes );

            return Json( issues, JsonRequestBehavior.AllowGet );
        }

        /* 
        =============================================================================================================================================
        Helper functions for various searches
        =============================================================================================================================================
        */

        private List<string> search_orgs_and_cats(string term)
        {
            //var qu = from org in dc.Orgs
            //         from cat in dc.Categories
            //         where org.orgName.StartsWith(term)
            //         where cat.orgId == org.Id 
            //         select (org.orgName + "-" + cat.catName);

            List<string> finalQuery = new List<string>();
            var quOrgs = searchService.MatchOrgsByName(term);
            
            foreach(var o in quOrgs)
            {
                var orgCats = productService.GetProductsByOrg(o.Id).ToList();
                if(orgCats.Count > 0)
                {
                    foreach(var c in orgCats)
                    {
                        if (o.orgName == c.ProductName)
                            finalQuery.Add(c.ProductName);
                        else
                            finalQuery.Add(o.orgName + "-" + c.ProductName);
                    }
                }
            }

            return finalQuery;
        }

        private List<string> search_users(string term)
        {

            //var qu = from user in dc.AspNetUsers
            //              where user.DisplayName.StartsWith(term)
            //              select user;

            var qu = searchService.MatchUsersByName(term);

            return qu.Select(u => u.DisplayName).ToList();

        }

        private List<Org> search_orgs(string term)
        {

            var qu = searchService.MatchOrgsByName(term);

            return qu.ToList();

        }

        private List<Issue> search_issues(string term)
        {

            var qu = searchService.MatchIssuesByTitle(term);
            return qu.ToList();
        }
    }
}