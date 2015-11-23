using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using Sigil.Services;

namespace Sigil.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrgService orgService;
        private readonly IErrorService errorService;

       public AdminController(IOrgService orgS, IErrorService errS) {
            orgService = orgS;
            errorService = errS;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrgApplicants()
        {
            var newOrgs = orgService.GetAllOrgApplicants();//dc.OrgApps.Select(o => o).ToList();

            return View(newOrgs);
        }

        public ActionResult ErrorLog()
        {
            var errors = errorService.GetAllErrors();//dc.Errors.Select(e => e).ToList();
            return View(errors);
        }
    }
}