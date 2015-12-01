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

        public SharedController( IErrorService errS, 
                                 IUserService userS, 
                                 ISubscriptionService subS, 
                                 IOrgService orgS, 
                                 IProductService prodS ) {
            errorService = errS;
            userService = userS;
            subscriptionService = subS;
            orgService = orgS;
            productService = prodS;
        }

        public ActionResult _SideBar() {
            SideBarVM sidebarVM = new SideBarVM().Init();
            
            string controller = Request.RequestContext.RouteData.Values[ "controller" ].ToString();

            if (controller == "Org" || controller == "Issue" || controller == "Product") {
                sidebarVM.showOrgBox = true;

                var currentURL = Request.Url.AbsoluteUri.Split('/');
                // Get the org
                sidebarVM.thisOrg = orgService.GetOrg(currentURL[3]);
                // Get the org's products
                sidebarVM.orgProducts = productService.GetProductsByOrg(sidebarVM.thisOrg.Id);

                //System.Web.Routing.RouteValueDictionary r = Request.RequestContext.RouteData.Values;
            }

            if (Request.IsAuthenticated) {
                sidebarVM.showSubscriptions = true;
                
                List<Subscription> subs = subscriptionService.GetUserSubscriptions( User.Identity.GetUserId() ).ToList();
                sidebarVM.Subscriptions = subs.Select(s => new SubscriptionViewModel().Create(s));
            }

            return PartialView( sidebarVM );
        }
    }
}