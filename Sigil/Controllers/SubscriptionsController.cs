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
        private readonly IProductService productService;
        private readonly ITopicService topicService;
        private readonly ICountService countDataService;

        public SubscriptionsController(IOrgService orgS, IIssueService issS, ISubscriptionService subS, IErrorService errS, ICountService countS, IProductService prodS, ITopicService topS)
        {
            orgService = orgS;
            issueService = issS;
            subscriptionService = subS;
            errorService = errS;
            countDataService = countS;
            productService = prodS;
            topicService = topS;
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
        public ActionResult AddSubscription(string URL, string type)
        {
            var userid = User.Identity.GetUserId();
            Subscription new_sub = new Subscription();
            new_sub.UserId = userid;
            
            if(type == "topic")
            {
                //then its a topic....
                var topic = topicService.GetTopic(URL);
                new_sub.TopicId = topic.Id;
            }
            else if(type == "product")
            {
                //then its a product
                var product = productService.GetProduct(URL, true); // products are the only ones who subscribe by name instead of url

                if (product.ProductURL == "Default")
                {
                    new_sub.OrgId = product.OrgId;
                }
                else
                {
                    new_sub.ProductId = product.Id;
                }
                countDataService.UpdateOrgSubscriptionCount(product.OrgId);
                countDataService.SaveOrgDataChanges();
            }
            else if(type == "org")
            {
                var org = orgService.GetOrg(URL);
                new_sub.OrgId = org.Id;
                countDataService.UpdateOrgSubscriptionCount(org.Id);
                countDataService.SaveOrgDataChanges();
            }

            try
            {
                subscriptionService.CreateSubscription(new_sub);
                subscriptionService.SaveSubscription();
                
            }
            catch (Exception e)
            {
                errorService.CreateError(new_sub, e);
            }
            return new EmptyResult();
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteSubscription(string URL, string type)
        {
            var userId = User.Identity.GetUserId();
            Subscription sub = default(Subscription);
            if (type == "topic")
            {
                var topic = topicService.GetTopic(URL);
                sub = subscriptionService.GetUserTopicSubscription(userId, topic.Id);
            }
            else if(type == "product")
            {
                var product = productService.GetProduct(URL, true); // products are the only ones who subscribe by name instead of url
                if (product.ProductURL == "Default")
                {
                    sub = subscriptionService.GetUserOrgSubscription(userId, product.OrgId);
                }
                else
                {
                    sub = subscriptionService.GetUserProductSubscription(userId, product.Id);
                }

                countDataService.UpdateOrgSubscriptionCount(product.OrgId, false);
                countDataService.SaveOrgDataChanges();

            }
            else
            {
                var org = orgService.GetOrg(URL);
                sub = subscriptionService.GetUserOrgSubscription(userId, org.Id);
                countDataService.UpdateOrgSubscriptionCount(org.Id, false);
                countDataService.SaveOrgDataChanges();
            }

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
