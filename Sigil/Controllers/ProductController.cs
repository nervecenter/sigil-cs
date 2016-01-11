using Microsoft.AspNet.Identity;
using PagedList;
using Sigil.Models;
using Sigil.Services;
using Sigil.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sigil.Controllers
{
    public class ProductController : Controller
    {
        private readonly IOrgService orgService;
        private readonly IIssueService issueService;
        private readonly ICommentService commentService;
        private readonly IUserService userService;
        private readonly ICountService countDataService;
        private readonly IErrorService errorService;
        private readonly IProductService productService;


        public ProductController(IOrgService orgS, IIssueService issS, ICommentService comS, IUserService userS, ICountService countS, IErrorService errS, IProductService prodS)
        {
            orgService = orgS;
            issueService = issS;
            commentService = comS;
            userService = userS;
            countDataService = countS;
            errorService = errS;
            productService = prodS;
        }

        public ActionResult ProductPage(string orgURL, string productURL, int? page)
        {
            // Get the org
            Org thisOrg = orgService.GetOrg(orgURL);

            Product thisProduct = productService.GetProduct(productURL);

            // If the org doesn't exist, redirect to 404
            if (thisOrg == default(Org) || thisProduct == default(Product))
            {
                return RedirectToRoute("404");
            }
            else if(thisProduct.ProductURL == "Default")
            {
                return RedirectToAction("OrgPage", "Org", new { orgURL = thisOrg.orgURL });
            }

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserViewModel userVM = new UserViewModel().emptyUser();

            if (userId != null)
            {
                userVM = userService.GetUserViewModel(userId);
            }

            // MODEL: Put the org and the list of issues into a tuple as our page model
            int num_results_per_page = 3;
            int pageNumber = (page ?? 1);

            ProductPageViewModel productVM = new ProductPageViewModel();
            productVM.thisOrg = thisOrg;
            productVM.thisProduct = thisProduct;
            productVM.UserVM = userVM;
            
            var ProductIssues = issueService.GetAllProductIssues(thisProduct.Id).Select(i => new IssuePanelPartialVM()
            {
                issue = i,
                InPanel = true,
                UserVoted = userVM.UserVotes.Check_Vote(i.Id, thisOrg.Id)
            }).ToList();

            productVM.productIssues = ProductIssues.ToPagedList(pageNumber, num_results_per_page);
           
            return View(productVM);
        }

        [HttpPost]
        [ActionName("ProductPage")]
        public ActionResult OrgPage_Post(string orgURL, string productURL, int? page)
        {
            // Get the org
            Org thisOrg = orgService.GetOrg(orgURL);//dc.Orgs.FirstOrDefault(o => o.orgURL == orgURL);

            string issueTitle = Request.Form["title"];
            string issuetext = Request.Form["text"];
            // If the org doesn't exist, redirect to 404
            if (thisOrg == null)
            {
                return RedirectToRoute("404");
            }

            Product prod = productService.GetProduct(Convert.ToInt32(Request.Form["product-select"]));

            AddIssueVM addIssueVM = new AddIssueVM() { org = thisOrg, product = prod, title = issueTitle, text = issuetext };

            TempData["vm"] = addIssueVM;
            //return View( "AddIssue", "Issue", addIssueVM );
            return RedirectToAction("AddIssue_Post", "Issue");
        }

    }
}