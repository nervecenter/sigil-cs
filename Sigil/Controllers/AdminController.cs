using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using Sigil.Services;
using Microsoft.AspNet.Identity;
using Sigil.ViewModels;
using System.Text.RegularExpressions;

namespace Sigil.Controllers
{
    public class AdminController : Controller
    {
        private readonly IOrgService orgService;
        private readonly IErrorService errorService;
        private readonly IUserService userService;
        private readonly IImageService imageService;
        private readonly ITopicService topicService;
        private readonly IIssueService issueService;
        private readonly IProductService productService;
        private Regex inputFilter;

        private readonly ApplicationUserManager _userManager;

        public AdminController(IOrgService orgS, IErrorService errS, IUserService userS, ApplicationUserManager userM, IImageService imgS, ITopicService topS, IIssueService issS, IProductService prodS)
        {
            orgService = orgS;
            errorService = errS;
            userService = userS;
            _userManager = userM;

            imageService = imgS;
            topicService = topS;
            issueService = issS;
            productService = prodS;
            inputFilter = new Regex("[^a-z]");
        }

        // GET: Admin
        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult Index()
        {
            //return the view based on if its Sigil or Org

            if(User.IsInRole("OrgSuperAdmin") || User.IsInRole("OrgAdmin"))
            {
                var user = userService.GetUser(User.Identity.GetUserId());
                var userOrg = orgService.GetOrg(user.OrgId);
                return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = userOrg.orgURL });
            }
            else if(User.IsInRole("SigilAdmin"))
            {
                return RedirectToAction("SigilAdminIndex", "Admin");
            }

            return View();
        }

        #region Org Admin

        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult OrgAdmin(string orgURL)
        {
            //need to validate user is apart of the org they are trying to access.

            Org thisOrg = orgService.GetOrg(orgURL);
            OrgAdminIndexViewModel orgAdminVM = new OrgAdminIndexViewModel()
            {
                thisOrg = thisOrg,
                thisOrgProducts = productService.GetProductsByOrg(thisOrg.Id)
            };

            ViewBag.Message = TempData["Message"];

            return View("OrgAdminIndex", orgAdminVM);
        }

        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult AddOrgProduct(string orgURL)
        {
            Org thisOrg = orgService.GetOrg(orgURL);
            string newProductName = Request.Form["product-name"];
            string newProductURL = inputFilter.Replace(Request.Form["product-url"].ToLower(), "");
            if(newProductName.Length < 5 || newProductURL.Length < 5)
            {
                TempData["Message"] = "Product Name and Product Handle must be minimum 5 characters.";
                return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = thisOrg.orgURL });
            }
            else if(productService.GetAllProducts().Where(p => p.ProductName == newProductName).Count() != 0)
            {
                TempData["Message"] = "Product Name already exists. Please choose another.";
                return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = thisOrg.orgURL });
            }
            else if(productService.GetAllProducts().Where(p => p.ProductURL == newProductURL).Count() != 0)
            {
                TempData["Message"] = "Product Handle already exists. Please choose another.";
                return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = thisOrg.orgURL });
            }
            

            Product newProduct = new Product();
            newProduct.OrgId = thisOrg.Id;
            newProduct.ProductName = Request.Form["product-name"];
            newProduct.ProductURL = Request.Form["product-url"];
            productService.CreateProduct(newProduct);
            productService.SaveProduct();

            newProduct = productService.GetProduct(Request.Form["product-url"]);
            Image newImage = imageService.AssignDefaultImage(newProduct.Id, ImageTypeOwner.Product);
            newProduct.ImageId = newImage.Id;
            productService.UpdateProduct(newProduct);
            productService.SaveProduct();

            return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = thisOrg.orgURL });
        }

        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin")]
        public ActionResult DeleteOrgProduct(string orgURL, string productId)
        {
            Product product_to_delete = productService.GetProduct(productId);
            if(product_to_delete.ProductURL == "Default")
                return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = orgURL });

            if (product_to_delete.Issues.Count > 0)
            {
                var defaultProduct = productService.GetDefaultProduct(product_to_delete.OrgId);
                var movingIssues = issueService.GetAllProductIssues(product_to_delete.Id);
                foreach (Issue i in movingIssues)
                    issueService.ChangeIssueProduct(i, defaultProduct.Id);

            }
            
            productService.DeleteProduct(product_to_delete);

            return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = orgURL });
        }

        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult ChangeProductURL(string orgURL, string productURL)
        {
            var product = productService.GetProduct(productURL);
            string newURL = inputFilter.Replace(Request.Form["newURL"].ToLower(), "");
  
            var unique = productService.GetAllProducts().Any(p => p.ProductURL.ToLower() == newURL.ToLower() || p.ProductURL.ToLower().Contains(newURL.ToLower()));
            if(newURL.Length < 4)
            {
                TempData["Message"] = "Product Handles must be minimum of 4 characters.";
            }
            else if (productURL == "Default")
            {
                TempData["Message"] = "Can not change default products url.";
            }
            else if (!unique) //we need to take the not here because we are looking for similar urls in the .Any call
            {
                productService.UpdateProductURL(product, newURL);
                return RedirectToAction("ProductAdminIndex", "Admin", routeValues: new { orgURL = orgURL, productURL = newURL});
            }
            else
            {
                TempData["Message"] = "URL has already been taken.";
            }

            return RedirectToAction("ProductAdminIndex", "Admin", routeValues: new { orgURL = orgURL, productURL = productURL });
        }

        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin")]
        public ActionResult OrgURLChange(string orgURL) //routes to here dont work for some reason
        {
            var thisOrg = orgService.GetOrg(orgURL);
            string newURL = Request.Form["newURL"];

            var unique = orgService.GetAllOrgs().Any(o => o.orgURL.ToLower() == newURL.ToLower() || o.orgURL.ToLower().Contains(newURL.ToLower()));

            if (!unique) //we need to take the not here because we are looking for similar urls in the .Any call
            {
                orgService.UpdateOrgURL(thisOrg, newURL);
            }
            else
            {
                ViewBag.Message = "URL has already been taken.";
            }

            return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = thisOrg.orgURL });
        }


        //[Authorize(Roles = "SigilAdmin OrgSuperAdmin")]
        //public ActionResult OrgNameChange(string orgURL, string newOrgName)
        //{

        //}
        //[Authorize(Roles = "SigilAdmin OrgSuperAdmin")]
        //public ActionResult AddOrgAdmin(string orgURL, string UserDisplayName)
        //{

        //}
        //[Authorize(Roles = "SigilAdmin OrgSuperAdmin")]
        //public ActionResult RemoveOrgAdmin(string orgURL, string UserDisplayName)
        //{

        //}
        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult UploadOrgBanner(string orgURL)
        {
            var org = orgService.GetOrg(orgURL);
            imageService.OrgBannerImageUpload(org, Request.Files[0]);

            return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = org.orgURL });
        }
        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult UploadOrgIcon100(string orgURL)
        {
            var org = orgService.GetOrg(orgURL);
            imageService.OrgIcon100ImageUpload(org, Request.Files[0]);

            return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = org.orgURL });
        }
        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult UploadOrgIcon20(string orgURL)
        {
            var org = orgService.GetOrg(orgURL);
            imageService.OrgIcon20ImageUpload(org, Request.Files[0]);

            return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = org.orgURL });
        }

        #endregion
        #region Product Admin

        [Authorize (Roles= "SigilAdmin, OrgAdmin, OrgSuperAdmin")]
        public ActionResult ProductAdminIndex(string orgURL, string productURL)
        {
            ProductAdminIndexViewModel vm = new ProductAdminIndexViewModel();
            vm.thisOrg = orgService.GetOrg(orgURL);
            vm.thisProduct = productService.GetProduct(productURL, vm.thisOrg.Id);

            ViewBag.Message = TempData["Message"];

            return View(vm);
        }

        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult UploadProductBanner(string orgURL, string productURL)
        {
            //var org = orgService.GetOrg(orgURL);
            var product = productService.GetProduct(productURL);
            imageService.ProductBannerImageUpload(product, Request.Files[0]);

            return RedirectToAction("ProductAdminIndex", "Admin", routeValues: new { orgURL = orgURL, productURL = productURL });
        }
        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult UploadProductIcon100(string orgURL, string productURL)
        {
            var product = productService.GetProduct(productURL);
            imageService.ProductIcon100ImageUpload(product, Request.Files[0]);

            return RedirectToAction("ProductAdminIndex", "Admin", routeValues: new { orgURL = orgURL, productURL = productURL });
        }
        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin, OrgAdmin")]
        public ActionResult UploadProductIcon20(string orgURL, string productURL)
        {
            var product = productService.GetProduct(productURL);
            imageService.ProductIcon20ImageUpload(product, Request.Files[0]);

            return RedirectToAction("ProductAdminIndex", "Admin", routeValues: new { orgURL = orgURL, productURL = productURL });
        }

        [Authorize(Roles = "SigilAdmin, OrgSuperAdmin")]
        public ActionResult ProductURLChange(string orgURL, string productURL) //routes to here dont work for some reason
        {
            var thisOrg = orgService.GetOrg(orgURL);
            string newURL = Request.Form["newURL"];

            var unique = orgService.GetAllOrgs().Any(o => o.orgURL.ToLower() == newURL.ToLower() || o.orgURL.ToLower().Contains(newURL.ToLower()));

            if (!unique) //we need to take the not here because we are looking for similar urls in the .Any call
            {
                orgService.UpdateOrgURL(thisOrg, newURL);
            }
            else
            {
                ViewBag.Message = "URL has already been taken.";
            }

            return RedirectToAction("OrgAdmin", "Admin", routeValues: new { orgURL = thisOrg.orgURL });
        }

        #endregion
        #region Sigil Admin
        //================================================ Topic Admin ================================================//

        [Authorize(Roles ="SigilAdmin")]
        public ActionResult SigilAdminIndex()
        {
            SigilAdminIndexViewModel sigilVM = new SigilAdminIndexViewModel();
            sigilVM.AllOrgsWithProduct = orgService.GetAllOrgs().Select(o => new Tuple<Org, int>(o, productService.GetProductsByOrg(o.Id).Count()));

            return View("SigilAdminIndex", sigilVM);
        }
        

        [Authorize(Roles = "SigilAdmin")]
        public ActionResult TopicAdmin()
        {
            var allTopics = topicService.GetAllTopics();

            ViewBag.AllTopics = allTopics.Select(t => new TopicInfoVM()
            {
                thisTopic = t,
                numberOfChildProducts = issueService.GetAllTopicIssues(t.Id).Count()
            });

            return View("TopicAdmin");
        }

        [Authorize(Roles = "SigilAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTopic(TopicCreateViewModel model)
        {
           
            Topic newTopic = new Topic();
            newTopic.topicName = model.topicName;
            newTopic.topicURL = model.topicURL;
            newTopic.lastAdded = DateTime.UtcNow;
            newTopic.views = 1;

            topicService.CreateTopic(newTopic);
            topicService.SaveTopic();

            newTopic.Image = imageService.AssignDefaultImage(newTopic.Id, ImageTypeOwner.Topic);
            topicService.UpdateTopic(newTopic);

            var allTopics = topicService.GetAllTopics();

            ViewBag.AllTopics = allTopics.Select(t => new TopicInfoVM()
            {
                thisTopic = t,
                numberOfChildProducts = issueService.GetAllTopicIssues(t.Id).Count()
            });

            return View("TopicAdmin");
        }


        [Authorize(Roles = "SigilAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignProductToTopic(string OrgAndProductName, string topicId)
        {
            var orgProductPair = OrgAndProductName.Split('-');

            var product = productService.GetProduct(orgProductPair[1], true);

            product.TopicId = Convert.ToInt32(topicId);
            productService.UpdateProduct(product);

            var allTopics = topicService.GetAllTopics();

            ViewBag.AllTopics = allTopics.Select(t => new TopicInfoVM()
            {
                thisTopic = t,
                numberOfChildProducts = issueService.GetAllTopicIssues(t.Id).Count()
            });

            return View("TopicAdmin");
        }


        //====================================================== Roles Admin ======================================================//

        [Authorize(Roles = "SigilAdmin")]
        public ActionResult RolesIndex()
        {
            ViewBag.Roles = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.AllRoles = userService.GetAllRoles();
            return View("RolesAdmin");
        }

        [Authorize(Roles = "SigilAdmin")]
        public ActionResult CreateRole()
        {
            if (Request.HttpMethod == "POST")
            {
                string newRoleName = Request.Form["roleName"];
                userService.CreateRole(newRoleName);
            }
            ViewBag.AllRoles = userService.GetAllRoles();
            return View("RoleAdmin");
        }

        [Authorize(Roles = "SigilAdmin")]
        public ActionResult DeleteRole(string roleName)
        {
            userService.DeleteRole(roleName);
            ViewBag.AllRoles = userService.GetAllRoles();
            return RedirectToAction("RolesAdmin");
        }

        [Authorize(Roles = "SigilAdmin")]
        public ActionResult AssignUserToRole(string userDisplayName, string roleName)
        {
            //var user = userService.GetUser(User.Identity.GetUserId());
            var user = userService.GetUserByDisplayName(userDisplayName);
            
            _userManager.AddToRole(user.Id, roleName);

            ViewBag.Roles = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.AllRoles = userService.GetAllRoles();
            return View("RolesAdmin");
        }

        [Authorize(Roles = "SigilAdmin")]
        public ActionResult DeleteUserFromRole(string userDisplayName, string roleName)
        {
            var user = userService.GetUserByDisplayName(userDisplayName);
            _userManager.RemoveFromRole(user.Id, roleName);

            ViewBag.Roles = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.AllRoles = userService.GetAllRoles();
            return View("RolesAdmin");
        }

        [Authorize(Roles = "SigilAdmin")]
        public ActionResult GetUserRoles(string userDisplayName)
        {
            var user = userService.GetUserByDisplayName(userDisplayName);
            ViewBag.RolesForThisUser = _userManager.GetRoles(user.Id);
            ViewBag.Roles = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.AllRoles = userService.GetAllRoles();
            return View("RolesAdmin");
        }

        //public ActionResult ManageUserRoles()
        //{
        //    // prepopulat roles for the view dropdown
        //    var list = userService.GetAllRoles().ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
        //    ViewBag.Roles = list;
        //    ViewBag.AllRoles = userService.GetAllRoles();
        //    return View("RolesAdmin");
        //}

        //======================================== Org Apps Admin ==========================================//
        [Authorize(Roles = "SigilAdmin")]
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
        #endregion
    }
}