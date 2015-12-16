using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace Sigil
{
    public class SearchFilter
    {
        private static string FilteredWordsFilePath = "/App_Data/FilteredWords.csv";

        private static Comparison<Tuple<Issue,int>> SearchRank = new Comparison<Tuple<Issue, int>>(Search_Rank_Compare);

        private List<string> FilteredWords;


        public SearchFilter()
        {
            FilteredWords = new StreamReader(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + FilteredWordsFilePath)).ReadToEnd().Split(',').ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="IssueList"></param>
        /// <returns></returns>
        public List<Tuple<Issue, int>> FindMatchingIssues(string terms, List<Issue> IssueList)
        {
            //First clean and filter search terms
            ConcurrentBag<string> cleanedFilteredTerms = CleanAndFilterText(terms);

            //if none of the terms passed the clean and filter then return empty
            if (cleanedFilteredTerms.IsEmpty)
                return new List<Tuple<Issue, int>>();

            ConcurrentBag<Tuple<Issue,int>> FinalIssues = new ConcurrentBag<Tuple<Issue,int>>(); //final list of matching issues with matching rank

            //for each issue, clean title and text and then compare to each search term.

            //TODO: Need to run timing test to make sure parallelization is done in correct places. 
            Parallel.ForEach(IssueList, (iss) =>
            {
                ConcurrentBag<string> IssueWords = CleanAndFilterText(iss.title + iss.text);

                //first see if there are any direct matches by taking the intersection of both lists of words
                var directMatches = IssueWords.AsParallel().Intersect(cleanedFilteredTerms.AsParallel()).ToList();

                ConcurrentBag<string> partialMatches = new ConcurrentBag<string>();

                //then find partial matches
                
                Parallel.ForEach(IssueWords, (IssueTerm) =>
                {
                    Parallel.ForEach(cleanedFilteredTerms, (searchTerm) =>
                    {
                        if(IssueTerm.Contains(searchTerm))
                        {
                            partialMatches.Add(searchTerm);
                        }
                    });
                });

                if(directMatches.Count() > 0 || partialMatches.Count() > 0)
                {
                    //Since we want direct matches to count for more than partial matches we get the rank by taking 80% of direct and 20% of partial (we just use ints though to avoid decimals)
                    int rank = directMatches.Count() * 8 + partialMatches.Count() * 2;
                    FinalIssues.Add(new Tuple<Issue, int>(iss, rank));
                }
            });

            var final = FinalIssues.ToList();
            final.Sort(SearchRank);
            return final;
            
        }


        public List<Tuple<Issue, int>> FindMatchingIssues(string terms, List<Tuple<Issue, List<OfficialResponse>>> issuesAndResponses)
        {
            //First clean and filter search terms
            ConcurrentBag<string> cleanedFilteredTerms = CleanAndFilterText(terms);

            //if none of the terms passed the clean and filter then return empty
            if (cleanedFilteredTerms.IsEmpty)
                return new List<Tuple<Issue, int>>();

            ConcurrentBag<Tuple<Issue, int>> FinalIssues = new ConcurrentBag<Tuple<Issue, int>>(); //final list of matching issues with matching rank

            //for each issue, clean title and text and then compare to each search term.

            //TODO: Need to run timing test to make sure parallelization is done in correct places. 
            Parallel.ForEach(issuesAndResponses, (iss) =>
            {
                string AllOfficialResponseText = iss.Item2.Select(o => o.text).ToString();
                ConcurrentBag<string> IssueWords = CleanAndFilterText(iss.Item1.title + iss.Item1.text + AllOfficialResponseText);

                //first see if there are any direct matches by taking the intersection of both lists of words
                var directMatches = IssueWords.AsParallel().Intersect(cleanedFilteredTerms.AsParallel()).ToList();

                ConcurrentBag<string> partialMatches = new ConcurrentBag<string>();

                //then find partial matches

                Parallel.ForEach(IssueWords, (IssueTerm) =>
                {
                    Parallel.ForEach(cleanedFilteredTerms, (searchTerm) =>
                    {
                        if (IssueTerm.Contains(searchTerm))
                        {
                            partialMatches.Add(searchTerm);
                        }
                    });
                });

                if (directMatches.Count() > 0 || partialMatches.Count() > 0)
                {
                    //Since we want direct matches to count for more than partial matches we get the rank by taking 80% of direct and 20% of partial (we just use ints though to avoid decimals)
                    int rank = directMatches.Count() * 8 + partialMatches.Count() * 2;
                    FinalIssues.Add(new Tuple<Issue, int>(iss.Item1, rank));
                }
            });

            var final = FinalIssues.ToList();
            final.Sort(SearchRank);
            return final;
        }

        private ConcurrentBag<string> CleanAndFilterText(string text)
        {
            var cleanedText =  new ConcurrentBag<string>(text.Trim().ToLower().Split(' ', ','));
            var FilteredText = new ConcurrentBag<string>();

            Parallel.ForEach(cleanedText, (term) =>
            {
                if(!FilteredWords.Contains(term))
                {
                    FilteredText.Add(term);
                }
            });

            return FilteredText;
        }


        private static int Search_Rank_Compare(Tuple<Issue,int> a , Tuple<Issue,int> b)
        {
            if (a.Item2 > b.Item2)
                return -1;
            else if (a.Item2 < b.Item2)
                return 1;
            else
                return 0;
        }
    }
}