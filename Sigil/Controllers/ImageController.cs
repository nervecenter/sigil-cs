using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using Microsoft.AspNet.Identity;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using System.IO;

namespace Sigil.Controllers
{
    public class ImageUploaderController : Controller
    {

        enum imgType
        {
            user_icon, org_icon_100, org_icon_20, banner
        };

        private static string org_folder_path = "C:/Sigil/Sigil/Images/Org/";
        private static string user_folder_path = "C:/Sigil/Sigil/Images/User/";
        private static string tmp_upload_path = "C:/Sigil/Sigil/Images/TMP/";
        private static string default_folder_path = "C:/Sigil/Sigil/Images/Default/";


        public ActionResult Index(bool result)
        {
            if (result)
            {
                ViewBag.upload_result = "File Uploaded!";
            }
            else
            {
                ViewBag.upload_result = "Upload Failed. Sigil has been notified of the error.";
            }

            return View("Manage");
        }



        public ActionResult User_Icon_Upload()
        {
            var img = Request.Files[0];
            if (img != null && img.ContentLength > 0)
            {
                var userid = User.Identity.GetUserId();
                string user = dc.AspNetUsers.SingleOrDefault(u => u.Id == userid).DisplayName;
                if (userid == null)
                {
                    ErrorHandler.Log_Error(userid, "No user for Userid", dc);
                    return Index(false);
                }

                string img_path = tmp_upload_path + user + "_" + img.FileName;

                img.SaveAs(img_path);
                string convertedImgPath = img_convert(img_path, user, imgType.user_icon);

                if (convertedImgPath != "")
                {
                    var userImg = dc.Images.SingleOrDefault(i => i.UserId == userid);
                    if (userImg == default(Sigil.Models.Image))
                    {
                        userImg = new Sigil.Models.Image();
                        userImg.UserId = userid;
                        userImg.icon_100 = user + "_100.png";


                        try
                        {
                            dc.Images.InsertOnSubmit(userImg);
                            dc.SubmitChanges();
                        }
                        catch (Exception e)
                        {
                            ErrorHandler.Log_Error(userImg, e, dc);
                        }
                    }

                }

                return RedirectToAction("Index", "Manage");
            }

            return new EmptyResult();
        }

        private string img_convert(string file, string owner, imgType type)
        {
            byte[] img_bytes = System.IO.File.ReadAllBytes(file);
            ISupportedImageFormat format = new PngFormat();
            Size size = new Size();

            if (type == imgType.user_icon)
                size = new Size(100, 100);

            using (MemoryStream inStream = new MemoryStream(img_bytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imgFact = new ImageFactory(preserveExifData: true))
                    {
                        imgFact.Load(inStream).Resize(size).Format(format).Save(outStream);
                    }

                    string final_path = "";
                    if (type == imgType.user_icon)
                    {
                        final_path = user_folder_path + owner + "_100.png";


                    }
                    else if (type == imgType.org_icon_100)
                    {

                    }
                    else if (type == imgType.org_icon_20)
                    {

                    }
                    else if (type == imgType.banner)
                    {

                    }
                    else
                    {
                        //Something went wrong
                    }

                    var final_img = System.IO.File.Create(final_path);
                    outStream.Seek(0, SeekOrigin.Begin);
                    outStream.CopyTo(final_img);
                    final_img.Close();

                    return final_path;
                }

            }
        }
    }


    public class ImageController<T> : Controller
    {
        private static SigilDBDataContext dc = new SigilDBDataContext();
        private static string default_folder_path = "/Images/Default/";
        private static string org_folder_path = "/Images/Org/";
        private static string user_folder_path = "/Images/User/";

        //=============================== Default Image Assignment Functions ====================================//
        /// <summary>
        /// Assigns randomly 1 of 5 possible default icons of size 100 to passed user
        /// </summary>
        /// <param name="userID">Id of user that is getting default image assigned.</param>
        public static void Assign_Default_Icon(string userID)
        {
            int defaultIMG = RNG.RandomNumber(1, 6);

            string IMG_PATH = "default" + defaultIMG + ".png";

            try
            {
                Sigil.Models.Image userImg = new Sigil.Models.Image();
                userImg.UserId = userID;
                userImg.icon_100 = IMG_PATH;
                dc.Images.InsertOnSubmit(userImg);
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorHandler.Log_Error(userID, e, dc);
            }
        }

        /// <summary>
        /// Assigns randomly 1 of 5 possible default icons of size 100 to passed org
        /// </summary>
        /// <param name="orgID">Id of org that is getting defaut image assigned.</param>
        public static void Assign_Default_Icon(int orgID)
        {
            int defaultIMG = RNG.RandomNumber(1, 6);

            string IMG_PATH = "default" + defaultIMG + ".png";

            try
            {
                Sigil.Models.Image Img = new Sigil.Models.Image();
                Img.OrgId = orgID;
                Img.icon_100 = IMG_PATH;
                dc.Images.InsertOnSubmit(Img);
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorHandler.Log_Error(orgID, e, dc);
            }
        }

        /// <summary>
        /// Assigns default icon of size 20 to passed id of either an org or Topic
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="caller">Either the Type Org or Topic</param>
        public static void Assign_Default_Icon20(int ID, T caller)
        {
            string IMG_PATH = "default20.png";

            try
            {
                Sigil.Models.Image Img = new Sigil.Models.Image();

                if (caller is Org)
                    Img.OrgId = ID;
                else if (caller is Topic)
                    Img.TopicId = ID;
                else
                {
                    throw new Exception("Passed caller is not an Org or Topic");
                }
                Img.icon_20 = IMG_PATH;
                dc.Images.InsertOnSubmit(Img);
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorHandler.Log_Error(ID, e, dc);
            }
        }

        /// <summary>
        /// Assigns 1 out of 2 possible default banners to passed in Org or Topic
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="caller"></param>
        public static void Assign_Default_Banner(int ID, T caller)
        {
            int defaultIMG = RNG.RandomNumber(1, 3);

            string IMG_PATH = "default_Banner" + defaultIMG + ".png";

            try
            {
                Sigil.Models.Image Img = new Sigil.Models.Image();

                if (caller is Org)
                    Img.OrgId = ID;
                else if (caller is Topic)
                    Img.TopicId = ID;
                else
                {
                    throw new Exception("Passed caller is not an Org or Topic");
                }

                Img.banner = IMG_PATH;
                dc.Images.InsertOnSubmit(Img);
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorHandler.Log_Error(ID, e, dc);
            }
        }

        //=============================== User Icon Functions ==================================================//

        public static string Get_Icon_100(string userId)
        {
            try
            {
                return user_folder_path + dc.Images.Single(i => i.UserId == userId).icon_100;
            }
            catch (Exception e)
            {
                if (!(e.InnerException is ArgumentNullException))
                {
                    ErrorHandler.Log_Error(userId, e, dc);
                }
                return default_folder_path + "default1.png";
            }
        }


        //============================= Org/Cat/Topic Functions ==============================================//


        public static string Get_Icon_20(T caller)
        {
            try
            {
                var entry = Get_DB_Entry(caller);
                return org_folder_path + entry.icon_20;
            }
            catch (Exception e)
            {
                if (!(e is ArgumentNullException))
                {
                    ErrorHandler.Log_Error(caller, e, dc);
                }
                return default_folder_path + "default20.png";
            }
        }

        public static string Get_Icon_100(T caller)
        {
            try
            {
                var entry = Get_DB_Entry(caller);
                return org_folder_path + entry.icon_100;
            }
            catch (Exception e)
            {
                if (!(e.InnerException is ArgumentNullException))
                {
                    ErrorHandler.Log_Error(caller, e, dc);
                }
                return default_folder_path + "default2.png";
            }
        }

        public static string Get_Banner(T caller)
        {
            try
            {
                var entry = Get_DB_Entry(caller);
                return org_folder_path + entry.banner;
            }
            catch (Exception e)
            {
                if (!(e.InnerException is ArgumentNullException))
                {
                    ErrorHandler.Log_Error(caller, e, dc);
                }
                return default_folder_path + "default_banner.png";
            }
        }



        private static Sigil.Models.Image Get_DB_Entry(T caller)
        {
            if (caller is Org)
            {
                Org c = (Org)Convert.ChangeType(caller, typeof(T));
                return dc.Images.Single(i => i.OrgId == c.Id);
            }
            else if (caller is Topic)
            {
                Topic c = (Topic)Convert.ChangeType(caller, typeof(T));
                return dc.Images.Single(i => i.TopicId == c.Id);
            }
            else if (caller is Category)
            {
                Category c = (Category)Convert.ChangeType(caller, typeof(T));
                return dc.Images.Single(i => i.CatId == c.Id);
            }
            throw new ArgumentNullException();
        }
    }
}