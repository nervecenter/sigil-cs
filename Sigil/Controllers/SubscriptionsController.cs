using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace Sigil.Controllers
{
    public class SubscriptionsController : Controller
    {
        private SigilDBDataContext dc;
        public SubscriptionsController()
        {
            dc = new SigilDBDataContext();
        }
        // GET: Subscriptions
        [Authorize]
        public ActionResult Index()
        {
            var userid = User.Identity.GetUserId();

            var orgs = dc.Orgs.ToList();
            var subs = dc.Subscriptions.Where(s => s.UserId == userid).ToList();

            Tuple<List<Org>, List<Subscription>> orgs_and_usersubs = new Tuple<List<Org>, List<Subscription>>(orgs, subs);
            return View(orgs_and_usersubs);
        }

        // GET: Subscriptions/Create

        [Authorize]
        [HttpPost]
        public ActionResult AddSubscription(string orgURL)
        {
            var userid = User.Identity.GetUserId();
            var orgid = dc.Orgs.SingleOrDefault<Org>(o => o.orgURL == orgURL);
            Subscription new_sub = new Subscription();
            new_sub.OrgId = orgid.Id;
            new_sub.UserId = userid;

            try
            {
                dc.Subscriptions.InsertOnSubmit(new_sub);
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write new sub \"%s\" to database:\n%s", new_sub, e.Message);
            }
            return new EmptyResult();
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteSubscription(string orgURL)
        {
            var userid = User.Identity.GetUserId();
            var org = dc.Orgs.SingleOrDefault<Org>(o => o.orgURL == orgURL);
            var sub = dc.Subscriptions.SingleOrDefault<Subscription>(s => s.OrgId == org.Id && s.UserId == userid);

            try {

                dc.Subscriptions.DeleteOnSubmit(sub);
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not delete sub \"%s\" to database:\n%s", sub, e.Message);
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
