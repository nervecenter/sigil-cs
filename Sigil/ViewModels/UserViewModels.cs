using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.ViewModels
{
    public class UserHomeViewModel
    {
        public ApplicationUser HomeUser { get; set; }

        public ICollection<string> Subscriptions { get; set; }
        public ICollection<IssueViewModel> UserIssues { get; set; }
    }

    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public UserVoteCol UserVotes { get; set; }

        public ICollection<Notification> UserNotifications { get; set; }
        public ICollection<SubscriptionViewModel> UserSubscriptions { get; set; }


        
    }

    public class LandingPageViewModel
    {
        public ICollection<OrgViewModel> TrendingOrgsandIssues { get; set; }
    }


    public class SubscriptionViewModel
    {
        public string subName { get; set; }
        public string subURL { get; set; }

        public string subIcon { get; set; }

        public SubscriptionViewModel(Subscription s)
        {
            if (s.TopicId != null)
            {
                subName = s.Topic.topicName;
                subURL = "/" + s.Topic.topicURL;
                subIcon = s.Topic.Image.icon_20;
            }
            else if (s.CatId != null)
            {
                subName = s.Org.orgName + "-" + s.Category.catName;
                subURL = "/" + s.Org.orgURL + "/" + s.Category.catURL;
                subIcon = s.Category.Image.icon_20;
            }
            else
            {
                subName = s.Org.orgName;
                subURL = "/" + s.Org.orgURL;
                subIcon = s.Org.Image.icon_20;
            }
        }
    }
}