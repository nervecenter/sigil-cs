using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.ViewModels
{

    public struct IssuePanelPartialVM
    {
        public Issue issue { get; set; }

        public bool UserVoted { get; set; }

        public bool InPanel { get; set; }
    }

    public struct UserViewModel
    {
        public ApplicationUser User { get; set; }
        public UserVoteCol UserVotes { get; set; }

        public IEnumerable<Notification> UserNotifications { get; set; }
        public IEnumerable<SubscriptionViewModel> UserSubscriptions { get; set; }
    }

    public struct SideBarVM
    {
        public Org thisOrg { get; set; }

        public IEnumerable<SubscriptionViewModel> Subscriptions { get; set; }
        
    }


    #region SharedViewModel Helpers

    public struct SubscriptionViewModel
    {
        public string subName { get; set; }
        public string subURL { get; set; }

        public string subIcon { get; set; }

        public SubscriptionViewModel Create(Subscription s)
        {
            if (s.TopicId != null)
            {
                subName = s.Topic.topicName;
                subURL = "/" + s.Topic.topicURL;
                subIcon = s.Topic.Image.icon_20;
            }
            else if (s.CatId != null)
            {
                subName = s.Org.orgName + "-" + s.Product.ProductName;
                subURL = "/" + s.Org.orgURL + "/" + s.Product.ProductURL;
                subIcon = s.Product.Image.icon_20;
            }
            else
            {
                subName = s.Org.orgName;
                subURL = "/" + s.Org.orgURL;
                subIcon = s.Org.Image.icon_20;
            }

            return this;
        }
    }

    #endregion
}
