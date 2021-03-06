using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Services;
using Sigil.Models;
using Sigil.ViewModels;
using Microsoft.AspNet.Identity;

namespace Sigil.Controllers
{
    public class SharedController : Controller
    {
        private readonly IErrorService errorService;
        private readonly IUserService userService;
        private readonly ISubscriptionService subscriptionService;
        private readonly IOrgService orgService;
        private readonly IProductService productService;

        public SharedController( IErrorService errS, IUserService userS, ISubscriptionService subS, IOrgService orgS, IProductService prodS )
        {
            errorService = errS;
            userService = userS;
            subscriptionService = subS;
            orgService = orgS;
            productService = prodS;
        }

        public ActionResult _SideBar()
        {
            SideBarVM sidebarVM = new SideBarVM().Init();
            
            string controller = Request.RequestContext.RouteData.Values[ "controller" ].ToString();
            string action = Request.RequestContext.RouteData.Values[ "action" ].ToString();
            
            if (action == "OrgList") {
                sidebarVM.showOrgBox = false;
            }

            else if (controller == "Org" || controller == "Issue" || controller == "Product") {
                sidebarVM.showOrgBox = true;

                var currentURL = Request.Url.AbsoluteUri.Split('/');
                // Get the org
                sidebarVM.thisOrg = orgService.GetOrg(currentURL[3].Split('?')[0]);
                // Get the org's products
                sidebarVM.orgProducts = productService.GetProductsByOrg(sidebarVM.thisOrg.Id);

                //System.Web.Routing.RouteValueDictionary r = Request.RequestContext.RouteData.Values;
            }

            if (Request.IsAuthenticated) {
                sidebarVM.showSubscriptions = true;

                sidebarVM.UserVM = userService.GetUserViewModel( User.Identity.GetUserId() );

                //List<Subscription> subs = subscriptionService.GetUserSubscriptions( User.Identity.GetUserId() ).ToList();
                //sidebarVM.Subscriptions = subs.Select(s => new SubscriptionViewModel().Create(s));
            }

            return PartialView( sidebarVM );
        }

        public ActionResult _LoginPartial()
        {
            string userId = User.Identity.GetUserId();
            if (userId == null || !Request.IsAuthenticated)
                return PartialView("_LoginPartial", new UserViewModel().emptyUser());
            var VM = userService.GetUserViewModel(userId);

            return PartialView("_LoginPartial", VM);
        }

        public ActionResult _404() {
            return PartialView( "_404" );
        }
    }
}