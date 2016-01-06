using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Sigil.Models;
using Sigil.ViewModels;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using PagedList;
using Sigil.Services;

namespace Sigil.Controllers {

    public class HomeController : Controller {
        
        //Comparison Object for sorting
        private static Comparison<Issue> Rank = new Comparison<Issue>(Issue_Compare);

        private readonly ISubscriptionService subscriptionService;
        private readonly IUserService userService;
        private readonly IIssueService issueService;
        private readonly INotificationService notificationService;

        public HomeController(ISubscriptionService subS, IUserService userS, IIssueService issS, INotificationService noteS)
        {
            subscriptionService = subS;
            userService = userS;
            issueService = issS;
            notificationService = noteS;
        }

        public ActionResult Index(int? page) {
            var userID = User.Identity.GetUserId();

            if (userID != null)
            {

                UserViewModel uservm = userService.GetUserViewModel(userID);

                Home_IndexViewModel vm = new Home_IndexViewModel();
                vm.UserVM = uservm;


                //get all the users issues based on their subscriptions
                var userIssues = issueService.GetAllUserIssues(userID);

                //NEED TO SORT JUST GET THE SORTED ISSUES FROM THE ISSUE SERVICE!!!!!!!!

                //sort the users issues by issue rank 
                userIssues.OrderBy(i => Rank);
                List<IssuePanelPartialVM> userIssuesVM = userIssues.Select(i => new IssuePanelPartialVM() { issue = i, UserVoted = uservm.UserVotes.Check_Vote(i.Id), InPanel = true }).ToList();
                int num_results_per_page = 6;
                int pageNumber = (page ?? 1);
                vm.UserIssues = userIssuesVM.ToPagedList(pageNumber, num_results_per_page);
                
                //Tuple<PagedList.IPagedList<Sigil.Models.Issue>, UserVoteCol> issuesANDvotes = new Tuple<PagedList.IPagedList<Sigil.Models.Issue>, UserVoteCol>(, userVotes);
                return View(vm);
            }
            else 
            {
                return LandingPage();
                
            }
        }

        public ActionResult LandingPage()
        {
            Home_LandingPageViewModel vm = new Home_LandingPageViewModel();
            vm.TrendingIssues = Get_Trending_Issues_With_Topics();
            

            return View("LandingPage", vm);
        }

        public ActionResult FeaturesPage() {

            return View( "FeaturesPage" );
        }

        public ActionResult Legal()
        {
            var userId = User.Identity.GetUserId();
            UserViewModel userVM = default(UserViewModel);
            if (userId != null)
            {
                userVM = userService.GetUserViewModel(userId);
            }
            
            return View(userVM);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }




        //==================================================================== Helper Functions =============================================================

        /// <summary>
        /// Returns all issues associated with the users passed in subscriptions.
        /// </summary>
        /// <param name="userID">Users AspNetUser.Id</param>
        /// <param name="userSubs">Query of all subscriptions assocaited with the user.</param>
        /// <returns></returns>
        //private IQueryable<Issue> Get_User_Issues(string userID, IQueryable<Subscription> userSubs)
        //{
        //    return dc.Issues.Where(i => userSubs.Any(s => s.OrgId == i.OrgId || (s.TopicId != 0 && s.TopicId == i.TopicId) || i.UserId == userID || (s.CatId != 0 && s.CatId == i.CatId)));
        //}


        /// <summary>
        /// Gets the Top 3 trending issues site wide. FOR THE LANDING PAGE ONLY
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IGrouping<Org, IssuePanelPartialVM>> Get_Trending_Issues_With_Topics()
        {
            var pretrending = issueService.GetAllIssues().ToList();

            pretrending.Sort(Rank);

            var trending = pretrending.Select(i => new IssuePanelPartialVM(){ issue = i, InPanel = false, UserVoted = false }).GroupBy(i => i.issue.Product.Org);

            return trending;
        }




        static DateTime test_date = new DateTime(2015, 7, 17, 13, 33, 57); //<------------ This will be changed once we go live.
        /// <summary>
        /// Calculates and returns the passed in issues rank. Based off the Reddit trending formula. 
        /// </summary>
        /// <param name="issue">Issue you want the rank of.</param>
        /// <returns>A double value with 7 decimal places</returns>
        private static double Calculate_Rank(Issue issue)
        {
            //Date of oldest submission as of right now ---> 7 / 17 / 2015 13:33:57
            TimeSpan secs_alive = issue.createTime - test_date;
            TimeSpan secs_voted = issue.lastVoted - issue.createTime;
            var r = Math.Round(Math.Log(issue.votes + 1) - Math.Log(Convert.ToDouble(issue.votes + issue.viewCount) + 2) + (secs_alive.TotalSeconds + secs_voted.TotalSeconds) / 19543, 7);
            return r;

        }




        /// <summary>
        /// Comparison function called by the comparison object for sorting lists. As of now sorts in decending order.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static int Issue_Compare(Issue x, Issue y)
        {
            var rank_x = Calculate_Rank(x);
            var rank_y = Calculate_Rank(y);

            if (rank_x == rank_y)
                return 0;
            else if (rank_x < rank_y)
                return 1;
            else
                return -1;
        }
    }
}