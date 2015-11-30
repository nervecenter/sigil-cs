using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Sigil.Services;

namespace Sigil.Controllers
{
    public class SubscriptionsController : Controller
    {

        private readonly IOrgService orgService;
        private readonly IIssueService issueService;
        private readonly ISubscriptionService subscriptionService;
        private readonly IErrorService errorService;
        private readonly ICountService countDataService;

        public SubscriptionsController(IOrgService orgS, IIssueService issS, ISubscriptionService subS, IErrorService errS, ICountService countS)
        {
            orgService = orgS;
            issueService = issS;
            subscriptionService = subS;
            errorService = errS;
            countDataService = countS;
        }

        // GET: Subscriptions
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var orgs = orgService.GetAllOrgs().ToList();//dc.Orgs.ToList();
            var subs = subscriptionService.GetUserSubscriptions(userId).ToList();//dc.Subscriptions.Where(s => s.UserId == userid).ToList();

            Tuple<List<Org>, List<Subscription>> orgs_and_usersubs = new Tuple<List<Org>, List<Subscription>>(orgs, subs);
            return View(orgs_and_usersubs);
        }

        // GET: Subscriptions/Create

        [Authorize]
        [HttpPost]
        public ActionResult AddSubscription(string orgURL)
        {
            var userid = User.Identity.GetUserId();
            var org = orgService.GetOrg(orgURL);
            Subscription new_sub = new Subscription();
            new_sub.OrgId = org.Id;
            new_sub.UserId = userid;

            countDataService.UpdateOrgSubscriptionCount(org.Id);
            
            try
            {
                subscriptionService.CreateSubscription(new_sub);
                subscriptionService.SaveSubscription();
                countDataService.SaveOrgDataChanges();
            }
            catch (Exception e)
            {
                errorService.CreateError(new_sub, e);
            }
            return new EmptyResult();
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteSubscription(string orgURL)
        {
            var userId = User.Identity.GetUserId();
            var org = orgService.GetOrg(orgURL);
            var sub = subscriptionService.GetUserSubscription(userId, org.Id);


            countDataService.UpdateOrgSubscriptionCount(org.Id, false);

            try
            {
                subscriptionService.RemoveSubscription(sub);
                countDataService.SaveOrgDataChanges();
            }
            catch (Exception e)
            {
                errorService.CreateError(sub, e);
            }
            return new EmptyResult();
        }

        // GET: Subscriptions/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}
