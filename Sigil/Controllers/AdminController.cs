﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using Sigil.Services;
using Microsoft.AspNet.Identity;

namespace Sigil.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrgService orgService;
        private readonly IErrorService errorService;
        private readonly IUserService userService;
        private readonly ApplicationUserManager _userManager;

        public AdminController(IOrgService orgS, IErrorService errS, IUserService userS, ApplicationUserManager userM)
        {
            orgService = orgS;
            errorService = errS;
            userService = userS;
            _userManager = userM;
        }

       public AdminController(IOrgService orgS, IErrorService errS) {
            orgService = orgS;
            errorService = errS;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RolesIndex()
        {
            var roles = userService.GetAllRoles();

            return View(roles);
        }

        public ActionResult CreateRole()
        {
            if (Request.HttpMethod == "POST")
            {
                string newRoleName = Request.Form["roleName"];
                userService.CreateRole(newRoleName);
            }
            return View();
        }

        public ActionResult DeleteRole(string roleName)
        {
            userService.DeleteRole(roleName);
            return RedirectToAction("RolesIndex");
        }

        public ActionResult AssignUserToRole(string userDisplayName, string roleName)
        {
            //var user = userService.GetUser(User.Identity.GetUserId());
            var user = userService.GetUserByDisplayName(userDisplayName);
            
            _userManager.AddToRole(user.Id, roleName);

            ViewBag.Roles = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();


            return View("ManageUserRoles");
        }

        public ActionResult DeleteUserFromRole(string userDisplayName, string roleName)
        {
            var user = userService.GetUserByDisplayName(userDisplayName);
            _userManager.RemoveFromRole(user.Id, roleName);

            ViewBag.Roles = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();


            return View("ManageUserRoles");
        }

        public ActionResult GetUserRoles(string userDisplayName)
        {
            var user = userService.GetUserByDisplayName(userDisplayName);
            ViewBag.RolesForThisUser = _userManager.GetRoles(user.Id);
            ViewBag.Roles = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            
            return View("ManageUserRoles");
        }

        //
        // GET: /Roles/Edit/5
        //public ActionResult EditRole(string roleName)
        //{
        //    var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

        //    return View(thisRole);
        //}

        ////
        //// POST: /Roles/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditRole(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        //{
        //    try
        //    {
        //        context.Entry(role).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        public ActionResult ManageUserRoles()
        {
            // prepopulat roles for the view dropdown
            var list = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string userId, string RoleName)
        {
            return RedirectToAction("ManageUserRoles");
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GetUserRoles(string userId)
        //{

        //    var userRoles = userService.GetUserRoles(userId);

        //    // prepopulat roles for the view dropdown
        //    var list = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
        //    ViewBag.Roles = list;
            

        //    return View("ManageUserRoles", userRoles);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteRoleForUser(string userId, string RoleName)
        //{

        //    //userService.RemoveUserRole(userId, RoleName);

        //    // prepopulat roles for the view dropdown
        //    var list = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
        //    ViewBag.Roles = list;

        //    return View("ManageUserRoles");
        //}

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