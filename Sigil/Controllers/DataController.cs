using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Sigil.Models;

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
        public static Tuple<int, int, int, int> Get_Sums(IEnumerable<ViewCountCol> views, IEnumerable<VoteCountCol> votes, IEnumerable<CommentCountCol> comms, IEnumerable<SubCountCol> subs, DateTime start, DateTime stop)
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

        public static List<Tuple<long, int>> Data_to_Google_Graph_Format(IEnumerable<CountCol> data, DateTime start, DateTime stop)
        {
            List<Tuple<long, int>> formatted_data = new List<Tuple<long, int>>();
            TimeSpan duriation = stop.Date - start.Date;

            for(int i = 0; i < duriation.Days; ++i)
            {
                int total = 0;
                foreach (var d in data)
                    total += d.Get_Value(start.AddDays(i + 1));
                formatted_data.Add(new Tuple<long, int>(DateTimeConversion.ToJSms(start.AddDays(i + 1)), total));
            }

            return formatted_data;
        }
        
        public static List<Tuple<long,int>> Data_to_Google_Graph_Format(CountCol data, DateTime start, DateTime stop)
        {
            List<Tuple<long, int>> formatted_data = new List<Tuple<long, int>>();
            TimeSpan duriation = stop.Date - start.Date;

            for(int i = 0; i < duriation.Days; ++i)
            {
                formatted_data.Add(new Tuple<long, int>(DateTimeConversion.ToJSms(start.AddDays(i + 1)), data.Get_Value(start.AddDays(i + 1))));
            }

            return formatted_data;
        }
        
        /// <summary>
        /// Gets the total number of unique comments for an issues. Unique is defined as different users
        /// </summary>
        /// <param name="allComments">A collection of comments for specified issue</param>
        /// <param name="start">Start datetime</param>
        /// <param name="stop">End datetime</param>
        /// <returns>Returns and interger that is the total number of unique commentors.</returns>
        internal static int Get_Unique_Count(IEnumerable<Comment> allComments, DateTime start, DateTime stop)
        {
            Dictionary<Tuple<string, int>, int> unique_users = new Dictionary<Tuple<string, int>, int>();
            int total = 0;
            foreach (var com in allComments)
            {
                var com_key = new Tuple<string, int>(com.UserId, com.issueId);
                if(com.createTime.Date >= start.Date && com.createTime.Date <= stop.Date && !unique_users.ContainsKey(com_key)) 
                {
                    unique_users.Add(com_key, 1);
                    total++;
                }
            }
            return total;
        }

        
        internal static List<Issue> Get_Under_Issues(IEnumerable<Issue> allIssues, DateTime start, DateTime stop, int v)
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

        internal static List<Issue> Get_Top_Issues(IEnumerable<Issue> allIssues, DateTime start, DateTime stop, int v)
        {
            return allIssues.Where(i => i.createTime.Date >= start.Date && i.createTime.Date <= stop.Date).OrderByDescending(i => i.votes).ThenByDescending(i => i.viewCount).Take(v).ToList();
        }

    }

}