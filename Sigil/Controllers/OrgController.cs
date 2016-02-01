using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using PagedList;
using Sigil.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Globalization;
using Sigil.Services;
using Sigil.ViewModels;
using AutoMapper;

namespace Sigil.Controllers
{
    public class OrgController : Controller
    {
        private readonly IOrgService orgService;
        private readonly ICountService countDataService;
        private readonly IIssueService issueService;
        private readonly ICommentService commentService;
        private readonly IUserService userService;
        private readonly IImageService imageService;
        private readonly ISubscriptionService subscriptionService;
        private readonly IProductService productService;

        /* 
        ==================== 
        OrgPage
  
            Returns a page listing the latest issues specific to an org 
        ==================== 
        */

        public OrgController(IOrgService orgS, ICountService countS, IIssueService issueS, ICommentService commS, IUserService userS, IImageService imgS, ISubscriptionService subS, IProductService prodS)
        {
            orgService = orgS;
            countDataService = countS;
            issueService = issueS;
            commentService = commS;
            userService = userS;
            imageService = imgS;
            subscriptionService = subS;
            productService = prodS;

        }

        public ActionResult OrgList() {
            // Get the org
            var allOrgs = orgService.GetAllOrgs();//dc.Orgs.FirstOrDefault(o => o.orgURL == orgURL);

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserViewModel userVM = new UserViewModel().emptyUser();

            if ( userId != null ) {
                userVM = userService.GetUserViewModel( userId );
            }

            OrgListViewModel orgListVM = new OrgListViewModel();
            orgListVM.UserVM = userVM;
            orgListVM.orgs = allOrgs;
            
            return View( orgListVM );
        }

        [HttpGet]
        public ActionResult OrgPage(string orgURL, int? page)
        {
            // Get the org
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault(o => o.orgURL == orgURL);

            // If the org doesn't exist, redirect to 404
            if (thisOrg == null)
            {
                return RedirectToRoute("404");
            }

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserViewModel userVM = new UserViewModel().emptyUser();

            if (userId != null)
            {
                userVM = userService.GetUserViewModel( userId );
            }
            //ViewBag.userVotes = userVotes;

            // MODEL: Put the org and the list of issues into a tuple as our page model
            int num_results_per_page = 20;
            int pageNumber = (page ?? 1);

            OrgPageViewModel orgVM = new OrgPageViewModel();
            orgVM.UserVM = userVM;
            orgVM.thisOrg = thisOrg;

            var OrgIssues = issueService.GetAllOrgIssues(thisOrg.Id).Select(i => new IssuePanelPartialVM()
            {
                issue = i,
                InPanel = true,
                UserVoted = userVM.UserVotes.Check_Vote(i.Id)
            }).ToList();

            orgVM.orgIssues = OrgIssues.ToPagedList(pageNumber, num_results_per_page);
            //Tuple<Org, UserViewModel, PagedList.IPagedList<Sigil.Models.Issue>> orgAndIssues = new Tuple<Org, UserViewModel, PagedList.IPagedList<Sigil.Models.Issue>>(thisOrg, userView, OrgIssues.ToPagedList(pageNumber, num_results_per_page));

            // This may not actually be necessary.
            //ViewBag.userSub = dc.Subscriptions.SingleOrDefault(s => s.UserId == userId && s.OrgId == thisOrg.Id);

            // Pass our org and issues to the view as the model
            return View(orgVM);
        }

        [HttpPost]
        [ActionName("OrgPage")]
        public ActionResult OrgPage_Post( string orgURL, int? page ) {
            // Get the org
            Org thisOrg = orgService.GetOrg( orgURL );//dc.Orgs.FirstOrDefault(o => o.orgURL == orgURL);

            string issueTitle = Request.Form[ "title" ];
            string issuetext = Request.Form["text"];
            // If the org doesn't exist, redirect to 404
            if ( thisOrg == null ) {
                return RedirectToRoute( "404" );
            }

            Product prod = productService.GetProduct(Convert.ToInt32(Request.Form["product-select"]));
            
            AddIssueVM addIssueVM = new AddIssueVM() { org = thisOrg, product = prod, title = issueTitle, text = issuetext };
          
            TempData[ "vm" ] = addIssueVM;
            //return View( "AddIssue", "Issue", addIssueVM );
            return RedirectToAction( "AddIssue_Post", "Issue");
        }


        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult DeleteIssue(string orgURL, int issueId)
        {
            var user = userService.GetUser(User.Identity.GetUserId());

            var issue = issueService.GetIssue(issueId);
            var org = orgService.GetOrg(orgURL);

            if (user.OrgId == org.Id)
            {
                issueService.DeleteIssue(issue);
            }

            return RedirectToAction("OrgPage", "Org", routeValues: new { orgURL = org.orgURL });
        }

        /* 
        ==================== 
        OrgResponsesPage
  
            Returns a page listing the issues specific to an org with responses, sorted by date descending
        ==================== 
        */

        public ActionResult OrgResponses( string orgURL, int? page ) {
            // Get the org
            Org thisOrg = orgService.GetOrg( orgURL );//dc.Orgs.FirstOrDefault(o => o.orgURL == orgURL);

            // If the org doesn't exist, redirect to 404
            if ( thisOrg == null ) {
                return RedirectToRoute( "404" );
            }

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserViewModel userVM = new UserViewModel().emptyUser();

            if ( userId != null ) {
                userVM = userService.GetUserViewModel( userId );
            }
            //ViewBag.userVotes = userVotes;

            // MODEL: Put the org and the list of issues into a tuple as our page model
            int num_results_per_page = 20;
            int pageNumber = ( page ?? 1 );

            OrgResponsesViewModel orgResVM = new OrgResponsesViewModel();
            orgResVM.UserVM = userVM;
            orgResVM.thisOrg = thisOrg;

            var OrgRespondedIssues = issueService.GetAllOrgIssues( thisOrg.Id ).Where(i => i.responded || i.OfficialResponses.Count > 0).Select( i => new IssuePanelPartialVM() {
                issue = i,
                InPanel = true,
                UserVoted = userVM.UserVotes.Check_Vote( i.Id )
            } ).ToList();

            orgResVM.orgRespondedIssues = OrgRespondedIssues.ToPagedList( pageNumber, num_results_per_page );
            //Tuple<Org, UserViewModel, PagedList.IPagedList<Sigil.Models.Issue>> orgAndIssues = new Tuple<Org, UserViewModel, PagedList.IPagedList<Sigil.Models.Issue>>(thisOrg, userView, OrgIssues.ToPagedList(pageNumber, num_results_per_page));

            // This may not actually be necessary.
            //ViewBag.userSub = dc.Subscriptions.SingleOrDefault(s => s.UserId == userId && s.OrgId == thisOrg.Id);

            // Pass our org and issues to the view as the model
            return View( orgResVM );
        }

        /* 
        ==================== 
        OrgData
  
            Data page for an Org, showing views/votes this week/month, responsiveness, and top issues  
        ==================== 
        */
        [Authorize (Roles = "SigilAdmin, OrgAdmin, OrgSuperAdmin")]
        public JsonResult DefaultData(string orgURL)
        {
            Org thisOrg = orgService.GetOrg(orgURL);

            var orgIssueViewData = countDataService.GetOrgViewCountCols(thisOrg.Id);

            var View_Data = DataVisualization.Data_to_Google_Graph_Format(orgIssueViewData, DateTime.Now.AddDays(-7), DateTime.Now);

            return Json(View_Data.Select(d => new { viewDate = d.Item1, viewCount = d.Item2 }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CustomData(string orgURL, string dataType, string start, string stop)
        {
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);

            DateTime startDate = DateTimeConversion.FromJSms(start);

            DateTime stopDate = DateTimeConversion.FromJSms(stop);

            List<Tuple<long, int>> view_data = new List<Tuple<long, int>>();

            switch(dataType)
            {
                case "Views":
                    {
                        var data = countDataService.GetOrgViewCountCols(thisOrg.Id);//dc.ViewCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<ViewCountCol>.XMLtoDATA(v.count)); 
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Votes":
                    {
                        var data = countDataService.GetOrgVoteCountCols(thisOrg.Id);//dc.VoteCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<VoteCountCol>.XMLtoDATA(v.count));
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Comments":
                    {
                        var data = countDataService.GetOrgCommentCountCols(thisOrg.Id);//dc.CommentCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<CommentCountCol>.XMLtoDATA(v.count));
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "Subscriptions":
                    {
                        var data = countDataService.GetOrgSubscriptionCount(thisOrg.Id);//dc.SubCounts.Where(vc => vc.OrgId == thisOrg.Id).Select(v => CountXML<SubCountCol>.XMLtoDATA(v.count));
                        view_data = DataVisualization.Data_to_Google_Graph_Format(data, startDate, stopDate);
                        break;
                    }
                case "All":
                    {
                        break;
                    }
            }

            return Json(view_data.Select(d => new { viewDate = d.Item1, viewCount = d.Item2 }), JsonRequestBehavior.AllowGet);
        }
        [Authorize (Roles ="SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult OrgData(string orgURL)
        {
            // Get the org
            Org thisOrg = orgService.GetOrg(orgURL); //dc.Orgs.FirstOrDefault<Org>(o => o.orgURL == orgURL);
            var user = userService.GetUser(User.Identity.GetUserId());
            var userVM = userService.GetUserViewModel(user.Id);
            if (user.OrgId != thisOrg.Id && !User.IsInRole("SigilAdmin"))
            {
                var usersOrg = orgService.GetOrg(user.OrgId);
                return RedirectToAction("OrgData", "Org", new { orgURL = usersOrg.orgURL });
            }

            OrgDataPageViewModel vm = new OrgDataPageViewModel();
            vm.thisOrg = thisOrg;
            vm.TopIssuePeriod = issueService.GetAllOrgIssues(thisOrg.Id).Where(i => i.editTime >= DateTime.Now.AddDays(-7)).OrderBy(o => o.votes).ThenBy(o => o.viewCount).Take(5).Select(i => new IssuePanelPartialVM()
            {
                issue = i,
                InPanel = true,
                UserVoted = userVM.UserVotes.Check_Vote(i.Id)
            }).ToList();

            vm.TopNewRising = issueService.GetAllOrgIssues(thisOrg.Id).OrderBy(i => i.editTime).ThenBy(i => i.viewCount).Take(5).Select(i => new IssuePanelPartialVM()
            {
                issue = i,
                InPanel = true,
                UserVoted = userVM.UserVotes.Check_Vote(i.Id)
            }).ToList();

            vm.TopWaitingResponse = issueService.GetAllOrgIssues(thisOrg.Id).Where(i => i.responded == false).OrderBy(i => i.viewCount).ThenBy(i => i.votes).Select(i => new IssuePanelPartialVM()
            {
                issue = i,
                InPanel = true,
                UserVoted = userVM.UserVotes.Check_Vote(i.Id)
            }).ToList();
            // Pass our model list of charts as the model of the view
            // return View(listOfCharts);
            return View(vm);
        }

    }
}