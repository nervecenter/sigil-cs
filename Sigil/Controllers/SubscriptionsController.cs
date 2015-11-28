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
            var org = orgService.GetOrg(orgURL);//dc.Orgs.SingleOrDefault<Org>(o => o.orgURL == orgURL);
            Subscription new_sub = new Subscription();
            new_sub.OrgId = org.Id;
            new_sub.UserId = userid;
            new_sub.ProductId = 0;
            new_sub.TopicId = 0;

            countDataService.UpdateOrgSubscriptionCount(org.Id);

            /*var subCs = dc.SubCounts.Single(s => s.OrgId == orgid.Id);
            var subData = CountXML<SubCountCol>.XMLtoDATA(subCs.count);
            subData.Update();
            subCs.count = CountXML<SubCountCol>.DATAtoXML(subData);*/
            
            try
            {
                subscriptionService.CreateSubscription(new_sub);
                subscriptionService.SaveSubscription();
                countDataService.SaveOrgDataChanges();
                //dc.Subscriptions.InsertOnSubmit(new_sub);
                //dc.SubmitChanges();
            }
            catch (Exception e)
            {
                errorService.CreateError(new_sub, e);
                //ErrorHandler.Log_Error(new_sub, e, dc);
                //Console.WriteLine("Could not write new sub \"%s\" to database:\n%s", new_sub, e.Message);
            }
            return new EmptyResult();
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteSubscription(string orgURL)
        {
            var userId = User.Identity.GetUserId();
            var org = orgService.GetOrg(orgURL);//dc.Orgs.SingleOrDefault<Org>(o => o.orgURL == orgURL);
            var sub = subscriptionService.GetUserSubscription(userId, org.Id);//dc.Subscriptions.SingleOrDefault<Subscription>(s => s.OrgId == org.Id && s.UserId == userid);


            countDataService.UpdateOrgSubscriptionCount(org.Id, false);
            /*var subCs = dc.SubCounts.Single(s => s.OrgId == org.Id);
            var subData = CountXML<SubCountCol>.XMLtoDATA(subCs.count);
            subData.Remove_Sub();
            subCs.count = CountXML<SubCountCol>.DATAtoXML(subData);*/

            try {

                //dc.Subscriptions.DeleteOnSubmit(sub);
                //dc.SubmitChanges();
                subscriptionService.RemoveSubscription(sub);
                countDataService.SaveOrgDataChanges();
            }
            catch (Exception e)
            {
                errorService.CreateError(sub, e);
                //ErrorHandler.Log_Error(sub, e, dc);
                //Console.WriteLine("Could not delete sub \"%s\" to database:\n%s", sub, e.Message);
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
