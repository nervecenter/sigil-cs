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
        public ActionResult Index()
        {
            IQueryable<Org> subs = from orgs in dc.Orgs
                                   select orgs;
            return View(subs);
        }

        // GET: Subscriptions/Create
        [Authorize]
        [HttpPost]
        public ActionResult addSubscription()
        {
            var userid = User.Identity.GetUserId();

            Subscription new_sub = new Subscription();
            new_sub.OrgId = Convert.ToInt32(Request.Form["org_id"]);
            new_sub.UserId = userid;

            try
            {
                dc.Subscriptions.InsertOnSubmit(new_sub);
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write issue \"%s\" to database:\n%s", new_sub.OrgId, e.Message);
            }
            return View();
        }


        // GET: Subscriptions/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

    }
}
