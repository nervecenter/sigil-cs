using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
namespace Sigil.Controllers
{
    public class AdminController : Controller
    {
        private SigilDBDataContext dc;
        public AdminController()
        {
            dc = new SigilDBDataContext();
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrgApplicants()
        {
            var newOrgs = dc.OrgApps.Select(o => o).ToList();

            return View(newOrgs);
        }

        public ActionResult ErrorLog()
        {
            var errors = dc.Errors.Select(e => e).ToList();
            return View(errors);
        }
    }
}