using Microsoft.AspNet.Identity;
using PagedList;
using Sigil.Models;
using Sigil.Services;
using Sigil.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sigil.Controllers
{
    public class TopicController : Controller
    {

        private readonly IOrgService orgService;
        private readonly IIssueService issueService;
        private readonly ICommentService commentService;
        private readonly IUserService userService;
        private readonly ICountService countDataService;
        private readonly IErrorService errorService;
        private readonly IProductService productService;
        private readonly ITopicService topicService;


        public TopicController(IOrgService orgS, IIssueService issS, ICommentService comS, IUserService userS, ICountService countS, IErrorService errS, IProductService prodS, ITopicService topS)
        {
            orgService = orgS;
            issueService = issS;
            commentService = comS;
            userService = userS;
            countDataService = countS;
            errorService = errS;
            productService = prodS;
            topicService = topS;
        }


        public ActionResult TopicPage(string topicURL, int? page)
        {
            Topic thisTopic = topicService.GetTopic(topicURL);

            if (thisTopic == default(Topic))
            {
                return RedirectToRoute("404");
            }

            // Get the user and their subscriptions
            var userId = User.Identity.GetUserId();
            UserViewModel userVM = new UserViewModel().emptyUser();

            if (userId != null)
            {
                userVM = userService.GetUserViewModel(userId);
            }

            // MODEL: Put the org and the list of issues into a tuple as our page model
            int num_results_per_page = 3;
            int pageNumber = (page ?? 1);

            TopicPageViewModel topicVM = new TopicPageViewModel();
            topicVM.thisTopic = thisTopic;
            topicVM.UserVM = userVM;


            var topicIssues = issueService.GetAllTopicIssues(thisTopic.Id).Select(i => new IssuePanelPartialVM()
            {
                InPanel = true,
                issue = i,
                UserVoted = userVM.UserVotes.Check_Vote(i.Id)
            }).ToList();

            topicVM.topicIssues = topicIssues.ToPagedList(pageNumber, num_results_per_page);

            return View(topicVM);

        }


    }
}