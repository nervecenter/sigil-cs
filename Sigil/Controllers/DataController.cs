using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageProcessor;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Data.SqlTypes;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using System.IO;

namespace Sigil
{
    /// <summary>
    /// Class that converts C# datetime objects to equivalent JS Date objects and JS Date objects to C# Datetime objects -- Idea taken from here http://stackoverflow.com/questions/2404247/datetime-to-javascript-date
    /// </summary>
    public static class DateTimeConversion
    {
        private static readonly long DatetimeMinTimeTicks =
           (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static long ToJSms(this DateTime dt)
        {
            return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
        }

        public static DateTime FromJSms(string ms)
        {
            return new DateTime(1970, 1, 1).AddTicks(Convert.ToInt64(ms) * 10000);
        }
    }

    /// <summary>
    /// Class that handles transforing Org data into correct format so that it can be visualized usings assiociated Graph API
    /// </summary>
    static class DataVisualization
    {
        /// <summary>
        /// Function used to get total sums of an Issues Views, Votes and Comments over a specificed duration
        /// </summary>
        /// <param name="views">Issues view count collection</param>
        /// <param name="votes">Issues vote count collection</param>
        /// <param name="comms">Issues comment count collection</param>
        /// <param name="start">Starting DateTime date</param>
        /// <param name="stop">Ending DateTime date</param>
        /// <returns> A tuple of three ints where item1 is total views, item2 is total votes, and item3 is total comments.</returns>
        public static Tuple<int, int, int> Get_Sums(ViewCountCol views, VoteCountCol votes, CommentCountCol comms, DateTime start, DateTime stop)
        {
            int total_views = 0;
            int total_votes = 0;
            int total_comms = 0;
            TimeSpan duriation = stop.Date - start.Date;
            for (int i = 0; i < duriation.Days; ++i)
            {
                total_views += views.Get_Value(start.AddDays(i+1));
                total_votes += votes.Get_Value(start.AddDays(i+1));
                total_comms += comms.Get_Value(start.AddDays(i + 1));
                
            }

            return new Tuple<int, int, int>(total_views, total_votes, total_comms);
        }

        /// <summary>
        /// Gets the total counts of Views, Votes and Comments over a specificed duration accross all issues of an Org 
        /// </summary>
        /// <param name="views"> Collection of org view counts</param>
        /// <param name="votes"> Collection of org vote counts</param>
        /// <param name="comms"> Collection of org comment counts</param>
        /// <param name="subs"> Collection of org sub counts</param>
        /// <param name="start"> Date to start counting at</param>
        /// <param name="stop"> Date to end counting at</param>
        /// <returns> A tuple of total counts in the order views, votes, comments, subscriptions</returns>
        public static Tuple<int, int, int, int> Get_Sums(IQueryable<ViewCountCol> views, IQueryable<VoteCountCol> votes, IQueryable<CommentCountCol> comms, IQueryable<SubCountCol> subs, DateTime start, DateTime stop)
        {
            int total_views = 0;
            int total_votes = 0;
            int total_comms = 0;
            int total_subs = 0;
            TimeSpan duriation = stop.Date - start.Date;
            for (int i = 0; i < duriation.Days; ++i)
            {
                foreach (var vi in views)
                    total_views += vi.Get_Value(start.AddDays(i + 1));
                foreach (var vo in votes)
                    total_votes += vo.Get_Value(start.AddDays(i + 1));
                foreach (var co in comms)
                    total_comms += co.Get_Value(start.AddDays(i + 1));
                foreach (var su in subs)
                    total_subs += su.Get_Value(start.AddDays(i + 1));

            }

            return new Tuple<int, int, int, int>(total_views, total_votes, total_comms, total_subs);
        }

        public static List<Tuple<long, int>> Data_to_Google_Graph_Format(IQueryable<CountCol> data, DateTime start, DateTime stop)
        {
            List<Tuple<long, int>> formated_data = new List<Tuple<long, int>>();
            TimeSpan duriation = stop.Date - start.Date;

            for(int i = 0; i < duriation.Days; ++i)
            {
                int total = 0;
                foreach (var d in data)
                    total += d.Get_Value(start.AddDays(i + 1));
                formated_data.Add(new Tuple<long, int>(DateTimeConversion.ToJSms(start.AddDays(i + 1)), total));
            }

            return formated_data;
        }
        

        //public static Highcharts Create_Highchart(ViewCountCol views, VoteCountCol votes, CommentCountCol comms, DateTime start, DateTime stop, string chartName, string chartTitle)
        //{
        //    TimeSpan duriation = stop.Date - start.Date;
        //    Tuple<List<int>, List<int>, List<int>> voteViewdata = new Tuple<List<int>, List<int>, List<int>>(new List<int>(), new List<int>(), new List<int>());

        //    List<string> xA = new List<string>();

        //    for (int i = 0; i < duriation.Days; ++i)
        //    {

        //        voteViewdata.Item1.Add(votes.Get_Votes(start.AddDays(i + 1)));
        //        voteViewdata.Item2.Add(views.Get_Views(start.AddDays(i + 1)));
        //        voteViewdata.Item3.Add(comms.Get_Comments(start.AddDays(i + 1)));
        //        xA.Add(start.AddDays(i+1).ToShortDateString());
        //    }


        //    object[] viewSeriesObject = new object[voteViewdata.Item1.Count];
        //    object[] voteSeriesObject = new object[voteViewdata.Item2.Count];
        //    object[] commSeriesObject = new object[voteViewdata.Item3.Count];

        //    voteViewdata.Item1.ToArray().CopyTo(viewSeriesObject, 0);
        //    voteViewdata.Item2.ToArray().CopyTo(voteSeriesObject, 0);
        //    voteViewdata.Item3.ToArray().CopyTo(commSeriesObject, 0);

        //    var HChart = new DotNet.Highcharts.Highcharts(chartName);
        //    HChart.SetXAxis(new XAxis { Categories = xA.ToArray() });
        //    HChart.SetSeries(new Series[] { new Series { Data = new Data(viewSeriesObject), Name = "Views" },
        //                                    new Series { Data = new Data(voteSeriesObject), Name = "Votes" },
        //                                    new Series { Data = new Data(commSeriesObject), Name = "Comments" } });
        //    HChart.SetTitle(new Title { Text = chartTitle });

        //    return HChart;
        //}

        //public static Highcharts Create_Highchart(IQueryable<ViewCountCol> views, IQueryable<VoteCountCol> votes, IQueryable<CommentCountCol> comms, IQueryable<SubCountCol> subs, DateTime start, DateTime stop, string chartName, string chartTitle)
        //{
        //    TimeSpan duriation = stop.Date - start.Date;
        //    Tuple<List<int>, List<int>, List<int>, List<int>> voteViewdata = new Tuple<List<int>, List<int>, List<int>, List<int>>(new List<int>(), new List<int>(), new List<int>(), new List<int>());
        //    List<string> xA = new List<string>();
        //    for (int i = 0; i < duriation.Days; ++i)
        //    {
        //        int to_vo = 0;
        //        int to_vi = 0;
        //        int to_co = 0;
        //        int to_su = 0;
        //        foreach (var vo in votes)
        //            to_vo += vo.Get_Votes(start.AddDays(i+1));
        //        foreach (var vi in views)
        //            to_vi += vi.Get_Views(start.AddDays(i+1));
        //        foreach (var co in comms)
        //            to_co += co.Get_Comments(start.AddDays(i + 1));
        //        foreach (var su in subs)
        //            to_su += su.Get_Subs(start.AddDays(i + 1));
        //        voteViewdata.Item1.Add(to_vi);
        //        voteViewdata.Item2.Add(to_vo);
        //        voteViewdata.Item3.Add(to_co);
        //        voteViewdata.Item4.Add(to_su);
        //        xA.Add(start.AddDays(i+1).ToShortDateString());
        //    }

        //    object[] viewSeriesObject = new object[voteViewdata.Item1.Count];
        //    object[] voteSeriesObject = new object[voteViewdata.Item2.Count];
        //    object[] commSeriesObject = new object[voteViewdata.Item3.Count];
        //    object[] subsSeriesObject = new object[voteViewdata.Item4.Count];

        //    voteViewdata.Item1.ToArray().CopyTo(viewSeriesObject, 0);
        //    voteViewdata.Item2.ToArray().CopyTo(voteSeriesObject, 0);
        //    voteViewdata.Item3.ToArray().CopyTo(commSeriesObject, 0);
        //    voteViewdata.Item4.ToArray().CopyTo(subsSeriesObject, 0);

        //    var HChart = new DotNet.Highcharts.Highcharts(chartName);
        //    HChart.SetXAxis(new XAxis { Categories = xA.ToArray() });
        //    HChart.SetSeries(new Series[] { new Series { Data = new Data(viewSeriesObject), Name = "Views" },
        //                                    new Series { Data = new Data(voteSeriesObject), Name = "Votes" },
        //                                    new Series { Data = new Data(commSeriesObject), Name = "Comments" },
        //                                    new Series { Data = new Data(subsSeriesObject), Name = "Subscriptions" } });
        //    HChart.SetTitle(new Title { Text = chartTitle });

        //    return HChart;
        //}

        /// <summary>
        /// Gets the total number of unique comments for an issues. Unique is defined as different users
        /// </summary>
        /// <param name="allComments">A collection of comments for specified issue</param>
        /// <param name="start">Start datetime</param>
        /// <param name="stop">End datetime</param>
        /// <returns>Returns and interger that is the total number of unique commentors.</returns>
        internal static int Get_Unique_Count(IQueryable<Comment> allComments, DateTime start, DateTime stop)
        {
            Dictionary<Tuple<string, int>, int> unique_users = new Dictionary<Tuple<string, int>, int>();
            int total = 0;
            foreach (var com in allComments)
            {
                var com_key = new Tuple<string, int>(com.UserId, com.issueId);
                if(com.postDate.Date >= start.Date && com.postDate.Date <= stop.Date && !unique_users.ContainsKey(com_key)) 
                {
                    unique_users.Add(com_key, 1);
                    total++;
                }
            }
            return total;
        }

        
        internal static List<Issue> Get_Under_Issues(IQueryable<Issue> allIssues, DateTime start, DateTime stop, int v)
        {
            //underdog issues are the newest issues for now

            //Dictionary<int, List<double>> growth = new Dictionary<int, List<double>>();
            //TimeSpan duration = stop.Date - start.Date;
            //foreach(var iss in allIssues)
            //{
            //    List<double> ratios;
            //    for (int i = 0; i < duration.Days; ++i)
            //    {
            //        var viewCount = CountXML<ViewCountCol>.XMLtoDATA(views.Single(vi => vi.IssueId == iss.Id).count).Get_Views(start.AddDays(i + 1));
            //        var voteCount = CountXML<VoteCountCol>.XMLtoDATA(views.Single(vo => vo.IssueId == iss.Id).count).Get_Votes(start.AddDays(i + 1));
            //        double r = voteCount/viewCount;

            //    }
            //    growth.Add()
            //}

            return allIssues.Where(i => i.createTime.Date >= start.Date && i.createTime.Date <= stop.Date).OrderByDescending(i => i.createTime.Date).ThenByDescending(i => i.viewCount).Take(v).ToList();

        }

        internal static List<Issue> Get_Top_Issues(IQueryable<Issue> allIssues, DateTime start, DateTime stop, int v)
        {
            return allIssues.Where(i => i.createTime.Date >= start.Date && i.createTime.Date <= stop.Date).OrderByDescending(i => i.votes).ThenByDescending(i => i.viewCount).Take(v).ToList();
        }

    }

    namespace Controllers
    {
        public class ImageUploaderController : Controller
        {
            private SigilDBDataContext dc;

            enum imgType{
                user_icon, org_icon_100, org_icon_20, banner
            };

           

            private static string org_folder_path = "C:/Sigil/Sigil/Images/Org/";
            private static string user_folder_path = "C:/Sigil/Sigil/Images/User/";
            private static string tmp_upload_path = "C:/Sigil/Sigil/Images/TMP/";
            private static string default_folder_path = "C:/Sigil/Sigil/Images/Default/";

            public ImageUploaderController()
            {
                dc = new SigilDBDataContext();
            }

            public ActionResult Index(bool result)
            {
                if(result)
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
                if(img != null && img.ContentLength > 0)
                {
                    var userid = User.Identity.GetUserId();
                    string user = dc.AspNetUsers.SingleOrDefault(u => u.Id == userid).DisplayName;
                    if(userid == null)
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
                            catch(Exception e)
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
                        if(type == imgType.user_icon)
                        {
                            final_path = user_folder_path + owner + "_100.png";
                            

                        }
                        else if(type == imgType.org_icon_100)
                        {

                        }
                        else if(type == imgType.org_icon_20)
                        {

                        }
                        else if(type == imgType.banner)
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
            private static string default_folder_path = "../Images/Default/";
            private static string org_folder_path = "../Images/Org/";
            private static string user_folder_path = "../Images/User/";

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
                if(caller is Org)
                {
                    Org c = (Org)Convert.ChangeType(caller, typeof(T));
                    return dc.Images.Single(i => i.OrgId == c.Id);
                }
                else if(caller is Topic)
                {
                    Topic c = (Topic)Convert.ChangeType(caller, typeof(T));
                    return dc.Images.Single(i => i.TopicId == c.Id);
                }
                else if(caller is Category)
                {
                    Category c = (Category)Convert.ChangeType(caller, typeof(T));
                    return dc.Images.Single(i => i.CatId == c.Id);
                }
                throw new ArgumentNullException();
            }
        }
    }
}