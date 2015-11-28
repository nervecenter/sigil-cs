using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Sigil.Models;
using Sigil.Services;

namespace Sigil.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationManager _authManager;

        private readonly IOrgService orgService;
        private readonly ICountService countDataService;
        private readonly IErrorService errorService;
        private readonly IUserService userService;
        private readonly IImageService imageService;

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager,IAuthenticationManager authManager ,IOrgService orgS, ICountService countS, IErrorService errS, IUserService userS, IImageService imgS )
        {
            //UserManager = userManager;
            //SignInManager = signInManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _authManager = authManager;


            orgService = orgS;
            countDataService = countS;
            errorService = errS;
            userService = userS;
            imageService = imgS;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            //private set 
            //{ 
            //    _signInManager = value; 
            //}
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            //private set
            //{
            //    _userManager = value;
            //}
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _authManager;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    
                    Create_User_Extras(user.Id);

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Function that handles all setting up all the users extra database entries for the site.
        /// </summary>
        private void Create_User_Extras(string userID)
        {
            //ImageController<AspNetUser>.Assign_Default_Icon(userID);
            imageService.AssignDefaultImage(userID);
            //imageService.SaveImage();
            userService.CreateUserVote(userID);
            userService.SaveUserVotes();
        }

        [AllowAnonymous]
        public ActionResult OrgRegister()
        {
            return View();
        }

        /// <summary>
        /// Org Registration function used to add new Org to OrgApp database table for Sigil to review
        /// </summary>
        /// <param name="model">OrgRegistration model</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgRegister(OrgRegisterViewModel model)
        {
            OrgApp newOrg = new OrgApp();
            newOrg.orgName = model.orgName;
            newOrg.orgURL = model.orgURL;
            newOrg.username = model.DisplayName;
            newOrg.website = model.orgWebsite;
            newOrg.contact = model.orgContact;
            newOrg.comment = model.orgComment;
            newOrg.email = model.Email;
            newOrg.AdminName = model.orgAdminName;
            try
            {
                orgService.CreateOrgApp(newOrg);
                orgService.SaveOrgApp();
                //dc.OrgApps.InsertOnSubmit(newOrg);
                //dc.SubmitChanges();
            }
            catch(Exception e)
            {
                errorService.CreateError(newOrg, e, ErrorLevel.Critical);
                
                //ErrorHandler.Log_Error(newOrg, e, dc);
            }

            return View("LandingPage");
        }

        //need to protect this so that sigil is the only role allowed to call
        public async Task<ActionResult> OrgConfirmed(int norgID)
        {
            OrgApp verifiedOrg = orgService.ApproveOrgApp(norgID);//dc.OrgApps.Single(o => o.Id == norgID);



            var user = new ApplicationUser { UserName = verifiedOrg.username, Email = verifiedOrg.email };
            string tempPassword = Generate_Temp_Password();
            var result = await UserManager.CreateAsync(user, tempPassword);

            //var org_check = dc.Orgs.SingleOrDefault(o => o.orgURL == verifiedOrg.orgUrl);

            var newOrg = new Org();
            newOrg.orgName = verifiedOrg.orgName;
            newOrg.orgURL = verifiedOrg.orgURL;
            newOrg.UserID = CountXML<UserIDCol>.DATAtoXML(new UserIDCol(user.Id)).ToString();
            newOrg.lastView = DateTime.UtcNow;
            try
            {
                orgService.CreateOrg(newOrg);
                orgService.SaveOrg();
                //dc.Orgs.InsertOnSubmit(newOrg);
               // dc.SubmitChanges();
            }
            catch(Exception e)
            {
                errorService.CreateError(newOrg, e, ErrorLevel.Critical);
                
                //need to kick back to a new screen to have them try again
            }

            //Setup Org data collection db entries
            int orgID = orgService.GetOrg(newOrg.orgURL).Id;
            var orgProduct = new Product();
            orgProduct.ProductName = newOrg.orgName;
            orgProduct.ProductURL = "Default";
            orgProduct.ImageId = imageService.AssignDefaultImage(orgID, ImageType.Org);

            countDataService.CreateOrgCountData(orgID);
            countDataService.SaveOrgCountData();
            
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return RedirectToAction("Index", "Home");
        }



        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        //public static AspNetUser GetLoggedInUser( System.Security.Principal.IPrincipal user ) {
        //    string id = user.Identity.GetUserId();

        //    if ( id == null ) {
        //        return default( AspNetUser );
        //    } else {
        //        using ( SigilDBDataContext dc = new SigilDBDataContext() ) {
        //            return dc.AspNetUsers.Single<AspNetUser>( u => u.Id ==  id );
        //        }
        //    }
        //}

        //public static System.Collections.Generic.List<Subscription> userSubs( AspNetUser user ) {
        //    System.Collections.Generic.List<Subscription> subs;
        //    SigilDBDataContext dc = new SigilDBDataContext();
        //    subs = ( from Subs in dc.Subscriptions
        //             where Subs.UserId == user.Id
        //             select Subs ).ToList();
        //    return subs;
        //}

        //public static System.Collections.Generic.List<Subscription> userSubs( System.Security.Principal.IPrincipal user ) {
        //    System.Collections.Generic.List<Subscription> subs;
        //    SigilDBDataContext dc = new SigilDBDataContext();
        //    subs = ( from Subs in dc.Subscriptions
        //             where Subs.UserId == user.Identity.GetUserId()
        //             select Subs ).ToList();
        //    return subs;
        //}

        private string Generate_Temp_Password()
        {
            return Membership.GeneratePassword(8, 2);
        }

        /// <summary>
        /// Used to determine whether or not to show the IsOfficial comment check box when making comments. ---- NEED TO REFACTOR as well as add support for SigilAdmins as well
        /// </summary>
        /// <param name="userId"> UserId of the user that is looking at the issue page</param>
        /// <param name="orgId">OrgId of the issue currently being viewed</param>
        /// <returns> Either True if the user is an admin of the org or False if not.</returns>
        //public static bool CheckUserRole(string userId, int orgId)
        //{
        //    using (SigilDBDataContext dc = new SigilDBDataContext())
        //    {
        //        var user = dc.AspNetUsers.SingleOrDefault(u => u.Id == userId && u.orgId == orgId);
        //        if (user == default(AspNetUser))
        //            return false;
        //        else if (user.AspNetUserRoles.SingleOrDefault(r => r.AspNetRole.Rank <= (int)Role.OrgAdmin) != default(AspNetUserRole))
        //            return true;
        //        else
        //            return false;
        //    }
        //}
    }
}