using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace Sigil.ViewModels
{

    /// <summary>
    /// Used on Topic/TopicPage.cshtml && TopicController/TopicPage(string topicURL, int? page)
    /// </summary>
    public struct TopicPageViewModel
    {
        public Topic thisTopic { get; set; }

        public UserViewModel UserVM { get; set; }

        public IPagedList<IssuePanelPartialVM> topicIssues { get; set; }
    }

    /// <summary>
    /// Used on Admin/CreateTopic.cshtml && AdminController/CreateTopic
    /// </summary>
    public class TopicCreateViewModel
    {
        [Required]
        [Display(Name = "Topic Name")]
        public string topicName { get; set; }

        [Required]
        [StringLength(16, ErrorMessage ="The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Topic Url")]
        public string topicURL { get; set; }

    }

    /// <summary>
    /// Used on Admin/CreateTopic.cshtml && AdminController/CreateTopic
    /// </summary>
    public struct TopicInfoVM
    {
        public Topic thisTopic { get; set; }
        public int numberOfChildProducts { get; set; }
    }

}