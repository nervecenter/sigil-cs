using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using System.Data.SqlTypes;

namespace Sigil
{
    static class DataVisualization
    {
        public static Tuple<int, int> Get_Sum(ViewCountCol views, VoteCountCol votes, DateTime start, DateTime stop)
        {
            int total_views = 0;
            int total_votes = 0;

            TimeSpan duriation = stop.Date - start.Date;
            for (int i = 0; i < duriation.Days; ++i)
            {
                total_views += views.Get_Views(start.AddDays(i+1));
                total_votes += votes.Get_Votes(start.AddDays(i+1));
            }

            return new Tuple<int, int>(total_views, total_votes);
        }

        public static Tuple<int, int> Get_Sum(IQueryable<ViewCountCol> views, IQueryable<VoteCountCol> votes, DateTime start, DateTime stop)
        {
            int total_views = 0;
            int total_votes = 0;

            TimeSpan duriation = stop.Date - start.Date;
            for (int i = 0; i < duriation.Days; ++i)
            {
                foreach (var vi in views)
                    total_views += vi.Get_Views(start.AddDays(i+1));
                foreach (var vo in votes)
                    total_votes += vo.Get_Votes(start.AddDays(i+1));
            }

            return new Tuple<int, int>(total_views, total_votes);
        }

        public static Highcharts Create_Highchart(ViewCountCol views, VoteCountCol votes, DateTime start, DateTime stop, string chartName, string chartTitle)
        {
            TimeSpan duriation = stop.Date - start.Date;
            Tuple<List<int>, List<int>> voteViewdata = new Tuple<List<int>, List<int>>(new List<int>(), new List<int>());
            List<string> xA = new List<string>();
            for (int i = 0; i < duriation.Days; ++i)
            {
                
                voteViewdata.Item1.Add(votes.Get_Votes(start.AddDays(i+1)));
                voteViewdata.Item2.Add(views.Get_Views(start.AddDays(i+1)));
                xA.Add(start.AddDays(i+1).ToShortDateString());
            }


            object[] viewSeriesObject = new object[voteViewdata.Item1.Count];
            object[] voteSeriesObject = new object[voteViewdata.Item2.Count];

            voteViewdata.Item1.ToArray().CopyTo(viewSeriesObject, 0);
            voteViewdata.Item2.ToArray().CopyTo(voteSeriesObject, 0);

            var HChart = new DotNet.Highcharts.Highcharts(chartName);
            HChart.SetXAxis(new XAxis { Categories = xA.ToArray() });
            HChart.SetSeries(new Series[] { new Series { Data = new Data(viewSeriesObject), Name = "Views" },
                                            new Series { Data = new Data(voteSeriesObject), Name = "Votes" } });
            HChart.SetTitle(new Title { Text = chartTitle });

            return HChart;
        }

        public static Highcharts Create_Highchart(IQueryable<ViewCountCol> views, IQueryable<VoteCountCol> votes, DateTime start, DateTime stop, string chartName, string chartTitle)
        {
            TimeSpan duriation = stop.Date - start.Date;
            Tuple<List<int>, List<int>> voteViewdata = new Tuple<List<int>, List<int>>(new List<int>(), new List<int>());
            List<string> xA = new List<string>();
            for (int i = 0; i < duriation.Days; ++i)
            {
                int to_vo = 0;
                int to_vi = 0;

                foreach (var vo in votes)
                    to_vo += vo.Get_Votes(start.AddDays(i+1));
                foreach (var vi in views)
                    to_vi += vi.Get_Views(start.AddDays(i+1));

                voteViewdata.Item1.Add(to_vi);
                voteViewdata.Item2.Add(to_vo);
                xA.Add(start.AddDays(i+1).ToShortDateString());
            }

            object[] viewSeriesObject = new object[voteViewdata.Item1.Count];
            object[] voteSeriesObject = new object[voteViewdata.Item2.Count];

            voteViewdata.Item1.ToArray().CopyTo(viewSeriesObject, 0);
            voteViewdata.Item2.ToArray().CopyTo(voteSeriesObject, 0);

            var HChart = new DotNet.Highcharts.Highcharts(chartName);
            HChart.SetXAxis(new XAxis { Categories = xA.ToArray() });
            HChart.SetSeries(new Series[] { new Series { Data = new Data(viewSeriesObject), Name = "Views" },
                                            new Series { Data = new Data(voteSeriesObject), Name = "Votes" } });
            HChart.SetTitle(new Title { Text = chartTitle });

            return HChart;
        }
    }

    namespace Controllers
    {
        public class ImageController : Controller
        {
            private static SigilDBDataContext dc = new SigilDBDataContext();
            private static string default_img_path = "../Images/Default/";
            private static string org_folder_path = "../Images/Org/";
            private static string user_folder_path = "../Images/User/";



            public static string Get_Icon_15(int id, Type caller)
            {

                try
                {
                    var entry = Get_DB_Entry(id, caller);

                    return org_folder_path + entry.icon_15;
                }
                catch(Exception e)
                {
                    if (!(e.InnerException is ArgumentNullException))
                    {
                        ErrorHandler.Log_Error(id, e);
                    }


                    return default_img_path + "default_icon_15.png";
                    
                }
            }

            public static string Get_Icon_20(int id, Type caller)
            {
                try
                {
                    var entry = Get_DB_Entry(id, caller);

                    return org_folder_path + entry.icon_20;
                }
                catch (Exception e)
                {
                    if (!(e.InnerException is ArgumentNullException))
                    {
                        ErrorHandler.Log_Error(id, e);
                    }


                    return default_img_path + "default_icon_20.png";

                }
            }

            public static string Get_Icon_20(string id)
            {
                try
                {
                    return user_folder_path + dc.Images.Single(i => i.UserId == id).icon_20;
                }
                catch (Exception e)
                {
                    if (!(e.InnerException is ArgumentNullException))
                    {
                        ErrorHandler.Log_Error(id, e);
                    }


                    return default_img_path + "default_icon_20.png";

                }
            }

            public static string Get_Icon_100(int id, Type caller)
            {
                try
                {
                    var entry = Get_DB_Entry(id, caller);
                    return org_folder_path + entry.icon_100;
                }
                catch (Exception e)
                {
                    if (!(e.InnerException is ArgumentNullException))
                    {
                        ErrorHandler.Log_Error(id, e);
                    }


                    return default_img_path + "default_icon_100.png";

                }
            }
            
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
                        ErrorHandler.Log_Error(userId, e);
                    }


                    return default_img_path + "default_icon_100.png";

                }
            }

            public static string Get_Banner_Tall(int id, Type caller)
            {

                try
                {
                    var entry = Get_DB_Entry(id, caller);
                    return org_folder_path + entry.banner_tall;
                }
                catch (Exception e)
                {
                    if (!(e.InnerException is ArgumentNullException))
                    {
                        ErrorHandler.Log_Error(id, e);
                    }


                    return default_img_path + "default_banner_tall.png";

                }
            }

            public static string Get_Banner_Short(int id, Type caller)
            {
                try
                {
                    var entry = Get_DB_Entry(id, caller);
                    return org_folder_path + entry.banner_short;
                }
                catch (Exception e)
                {
                    if (!(e.InnerException is ArgumentNullException))
                    {
                        ErrorHandler.Log_Error(id, e);
                    }


                    return default_img_path + "default_banner_short.png";

                }
            }

            private static Image Get_DB_Entry(int id, Type c)
            {
                var t = c.ToString();
                switch (c.ToString())
                {
                    case "Sigil.Models.Org":
                        return dc.Images.Single(i => i.OrgId == id);
                    case "Sigil.Models.Topic":
                        return dc.Images.Single(i => i.TopicId == id);
                    case "Sigil.Models.Category":
                        return dc.Images.Single(i => i.CatId == id);
                    default:
                        throw new ArgumentNullException();
                }
            }


        }

    }
}