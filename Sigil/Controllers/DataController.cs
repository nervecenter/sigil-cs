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
                total_views += views.Get_Views(start.AddDays(i));
                total_votes += votes.Get_Votes(start.AddDays(i));
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
                foreach(var vi in views)
                    total_views += vi.Get_Views(start.AddDays(i));
                foreach(var vo in votes)
                    total_votes += vo.Get_Votes(start.AddDays(i));
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
                //voteViewdata.Add(new GraphData(start.AddDays(i), votes.Get_Votes(start.AddDays(i)), views.Get_Views(start.AddDays(i))));
                voteViewdata.Item1.Add(votes.Get_Votes(start.AddDays(i)));
                voteViewdata.Item2.Add(views.Get_Views(start.AddDays(i)));
                xA.Add(stop.AddDays(-i).ToShortDateString());
            }


            var HChart = new DotNet.Highcharts.Highcharts(chartName);
            HChart.SetXAxis(new XAxis { Categories = xA.ToArray() });
            HChart.SetSeries(new Series[] { new Series { Data = new Data(new object[] { voteViewdata.Item1.ToArray() }), Name = "Views" },
                                            new Series { Data = new Data(new object[] { voteViewdata.Item2.ToArray() }), Name = "Votes" } });
            HChart.SetTitle(new Title { Text = chartTitle });

            return HChart;
            //return new DotNet.Highcharts.Highcharts(chartName)
            //                .SetXAxis(new XAxis
            //                {
            //                    Categories = new[] { DateTime.Today.Date.AddDays(-6.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-5.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-4.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-3.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-2.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-1.0).ToShortDateString(),
            //                                         DateTime.Today.Date.ToShortDateString() }
            //                })
            //                .SetSeries(new Series[] {
            //                new Series {
            //                    Data = new Data(new object[] { weekOfViews[6],
            //                                                   weekOfViews[5],
            //                                                   weekOfViews[4],
            //                                                   weekOfViews[3],
            //                                                   weekOfViews[2],
            //                                                   weekOfViews[1],
            //                                                   weekOfViews[0] }),
            //                    Name = "Views"
            //                }, new Series {
            //                    Data = new Data(new object[] { weekOfVotes[6],
            //                                                   weekOfVotes[5],
            //                                                   weekOfVotes[4],
            //                                                   weekOfVotes[3],
            //                                                   weekOfVotes[2],
            //                                                   weekOfVotes[1],
            //                                                   weekOfVotes[0] }),
            //                    Name = "Votes"
            //                }
            //                })
            //                .SetTitle(new Title { Text = chartTitle });
        }

        public static Highcharts Create_Highchart(IQueryable<ViewCountCol> views, IQueryable<VoteCountCol> votes, DateTime start, DateTime stop, string chartName, string chartTitle)
        {
            TimeSpan duriation = stop.Date - start.Date;
            Tuple<List<int>, List<int>> voteViewdata = new Tuple<List<int>, List<int>>(new List<int>(), new List<int>());
            List<string> xA = new List<string>();
            for (int i = 0; i < duriation.Days; ++i)
            {
                //voteViewdata.Add(new GraphData(start.AddDays(i), votes.Get_Votes(start.AddDays(i)), views.Get_Views(start.AddDays(i))));

                //I was making this work with iqueryables of view/vote countcols for the org data page in the org controller
                
                int to_vo = 0;
                int to_vi = 0;

                foreach(var vo in votes )
                    to_vo += vo.Get_Votes(start.AddDays(i));
                foreach (var vi in views)
                    to_vi += vi.Get_Views(start.AddDays(i));

                voteViewdata.Item1.Add(to_vi);
                voteViewdata.Item2.Add(to_vo);
                xA.Add(stop.AddDays(-i).ToShortDateString());
            }


            var HChart = new DotNet.Highcharts.Highcharts(chartName);
            HChart.SetXAxis(new XAxis { Categories = xA.ToArray() });
            HChart.SetSeries(new Series[] { new Series { Data = new Data(new object[] { voteViewdata.Item1.ToArray() }), Name = "Views" },
                                            new Series { Data = new Data(new object[] { voteViewdata.Item2.ToArray() }), Name = "Votes" } });
            HChart.SetTitle(new Title { Text = chartTitle });

            return HChart;
            //return new DotNet.Highcharts.Highcharts(chartName)
            //                .SetXAxis(new XAxis
            //                {
            //                    Categories = new[] { DateTime.Today.Date.AddDays(-6.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-5.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-4.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-3.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-2.0).ToShortDateString(),
            //                                         DateTime.Today.Date.AddDays(-1.0).ToShortDateString(),
            //                                         DateTime.Today.Date.ToShortDateString() }
            //                })
            //                .SetSeries(new Series[] {
            //                new Series {
            //                    Data = new Data(new object[] { weekOfViews[6],
            //                                                   weekOfViews[5],
            //                                                   weekOfViews[4],
            //                                                   weekOfViews[3],
            //                                                   weekOfViews[2],
            //                                                   weekOfViews[1],
            //                                                   weekOfViews[0] }),
            //                    Name = "Views"
            //                }, new Series {
            //                    Data = new Data(new object[] { weekOfVotes[6],
            //                                                   weekOfVotes[5],
            //                                                   weekOfVotes[4],
            //                                                   weekOfVotes[3],
            //                                                   weekOfVotes[2],
            //                                                   weekOfVotes[1],
            //                                                   weekOfVotes[0] }),
            //                    Name = "Votes"
            //                }
            //                })
            //                .SetTitle(new Title { Text = chartTitle });
        }
    }
}