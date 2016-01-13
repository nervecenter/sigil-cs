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
using System.Web.Script.Serialization;
using Sigil.ViewModels;

namespace Sigil.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductService productService;
        private readonly ISearchService searchService;
        private readonly ICommentService commentService;
        private SearchFilter searchFilter;

        public SearchController(IProductService prodS, ISearchService searchS, ICommentService comS)
        {
            productService = prodS;
            searchService = searchS;
            commentService = comS;
            searchFilter = new SearchFilter();
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
                search_list["Topic"] = search_topics(term);
                search_list["Issue"] = search_issues(term);
                search_list["Product"] = search_products(term);
            }

            foreach(string k in search_list.Keys)
            {
                if(k == "Org")
                {
                    List<Org> found_orgs = (List<Org>)search_list[k];
                    foreach(Org o in found_orgs)
                    {
                        Final_search_list[o.orgName] = "/" + o.orgURL;
                    }
                }
                else if(k == "Issue")
                {
                    List<Issue> found_issues = (List<Issue>)search_list[k];
                    foreach (Issue i in found_issues)
                    {
                        Final_search_list[i.title] = "/" + i.Product.Org.orgURL + "/" + i.Product.ProductURL + "/" + i.Id;
                    }
                }
                else if( k == "Topic")
                {
                    List<Topic> found_topics = (List<Topic>)search_list[k];
                    foreach( Topic t in found_topics)
                    {
                        Final_search_list[t.topicName] = "/t/" + t.topicURL;
                    }
                }
                else if(k == "Product")
                {
                    List<Product> found_products = (List<Product>)search_list[k];
                    foreach (Product p in found_products)
                    {
                        Final_search_list[p.ProductName] = "/" + p.Org.orgURL + "/" + p.ProductURL;
                    }
                }
            }

            return Json(Final_search_list.Select(s => new { label = s.Key, value = s.Value }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchByUserDisplayName(string term)
        {
            var users = searchService.MatchUsersByName(term).Select(u => u.DisplayName).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AdminProductSearch(string term)
        {
            var products = productService.GetAllProducts();

            List<string> orgsandproducts = products.Where(p => p.ProductName.StartsWith(term)).Select(p => p.Org.orgName + '-' + p.ProductName).ToList();

            return Json(orgsandproducts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchOrgsAndProducts(string term)
        {
            List<string> search_list = new List<string>();
            if (!string.IsNullOrEmpty(term))
            {
                //search_list.AddRange(search_orgs(term).Select(o => o.orgName));
                search_list.AddRange(search_orgs_and_products(term));
            }

            return Json(search_list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchIssuesByOrg( int id, string term ) 
        {
            string partialsHTML = "";

            // Three cases: Term is empty; fetch all issues
            if (term == "") {
                var issues = searchService.MatchIssuesByOrg( id ).OrderByDescending( i => i.votes );

                foreach (Issue i in issues) {
                    if ( Request.IsAuthenticated ) {
                        // TODO: Make sure we check if the user voted on this issue
                        partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i, UserVoted = false, InPanel = true } );
                    } else {
                        partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i, UserVoted = false, InPanel = true } );
                    }
                }

                return Json( partialsHTML, JsonRequestBehavior.AllowGet );
            }

            // Otherwise, fetch matching issues

            

            var issuesList = searchService.MatchIssuesByOrg(id).ToList();//.Where( i => i.title.ToLower().Contains( term.ToLower() ) ).OrderByDescending( i => i.votes ).ToList();
            var issuesWithResponses = issuesList.Select(i => new Tuple<Issue, List<OfficialResponse>>(i, commentService.GetIssuesOfficialResponses(i.Id).ToList())).ToList();

            var MatchedIssues = searchFilter.FindMatchingIssues(term, issuesWithResponses);


             foreach (Tuple<Issue,int> i in MatchedIssues) {
                if ( Request.IsAuthenticated ) {
                    // TODO: Make sure we check if the user voted on this issue
                    partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i.Item1, UserVoted = false, InPanel = true } );
                } else {
                    partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i.Item1, UserVoted = false, InPanel = true } );
                }
            }

            if (partialsHTML == "") {
                partialsHTML += "No posts found matching your query.";
            }

            return Json( partialsHTML, JsonRequestBehavior.AllowGet );
        }

        [HttpPost]
        public JsonResult SearchIssuesByProduct( int id, string term ) {
            string partialsHTML = "";

            // Three cases: Term is empty; fetch all issues
            if ( term == "" ) {
                var issues = searchService.MatchIssuesByProduct( id ).OrderByDescending( i => i.votes );

                foreach ( Issue i in issues ) {
                    if ( Request.IsAuthenticated ) {
                        // TODO: Make sure we check if the user voted on this issue
                        partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i, UserVoted = false, InPanel = true } );
                    } else {
                        partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i, UserVoted = false, InPanel = true } );
                    }
                }

                return Json( partialsHTML, JsonRequestBehavior.AllowGet );
            }

            // Otherwise, fetch matching issues
            var issuesList = searchService.MatchIssuesByProduct( id ).Where( i => i.title.ToLower().Contains( term.ToLower() ) ).OrderByDescending( i => i.votes ).ToList();

            foreach ( Issue i in issuesList ) {
                if ( Request.IsAuthenticated ) {
                    // TODO: Make sure we check if the user voted on this issue
                    partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i, UserVoted = false, InPanel = true } );
                } else {
                    partialsHTML += IssuePanelPartialJsonVM.IssuePanelPartialHTML( new IssuePanelPartialVM { issue = i, UserVoted = false, InPanel = true } );
                }
            }

            if ( partialsHTML == "" ) {
                partialsHTML += "No posts found matching your query.";
            }

            return Json( partialsHTML, JsonRequestBehavior.AllowGet );
        }

        /* 
        =============================================================================================================================================
        Helper functions for various searches
        =============================================================================================================================================
        */

        private List<string> search_orgs_and_products(string term)
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
                var orgProducts = productService.GetProductsByOrg(o.Id).ToList();
                if(orgProducts.Count > 0)
                {
                    foreach(var p in orgProducts)
                    {
                        if (o.orgName == p.ProductName)
                            finalQuery.Add(p.ProductName);
                        else
                            finalQuery.Add(o.orgName + "-" + p.ProductName);
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

        private List<Topic> search_topics(string term)
        {
            var qu = searchService.MatchTopicsByName(term);
            return qu.ToList();
        }

        private List<Product> search_products(string term)
        {
            var qu = searchService.MatchProductsByName(term);
            return qu.ToList();
        }

        private List<Issue> search_issues(string term)
        {

            var qu = searchService.MatchIssuesByTitle(term);
            return qu.ToList();
        }
    }
}