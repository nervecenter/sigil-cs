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
    
    public struct IssuePanelPartialJsonVM
    {   
        public int issueID { get; set; }
        
        public int votes { get; set; }
        
        public string title { get; set; }
        
        public string text { get; set; }

        public string orgName { get; set; }

        public string orgIcon { get; set; }

        public string productName { get; set; }

        public string productIcon { get; set; }

        public string datePostedString { get; set; }

        public string userName { get; set; }

        public string userIcon { get; set; }

        public bool UserVoted { get; set; }

        public bool InPanel { get; set; }
    }

    public struct UserViewModel
    {
        public ApplicationUser User { get; set; }
        public UserVoteCol UserVotes { get; set; }

        public IEnumerable<Notification> UserNotifications { get; set; }
        public IEnumerable<SubscriptionViewModel> UserSubscriptions { get; set; }

        public UserViewModel emptyUser() {
            return new UserViewModel {
                UserVotes = new UserVoteCol(),
                UserNotifications = new List<Notification>(),
                UserSubscriptions = new List<SubscriptionViewModel>()
            };
        }
    }

    public struct SideBarVM
    {
        public bool showOrgBox { get; set; }

        public Org thisOrg { get; set; }

        public IEnumerable<Product> orgProducts { get; set; }

        public bool showSubscriptions { get; set; }

        public IEnumerable<SubscriptionViewModel> Subscriptions { get; set; }
        

        public SideBarVM Init()
        {
            showOrgBox = false;
            showSubscriptions = false;
            
            thisOrg = default(Org);

            orgProducts = new List<Product>().AsEnumerable();
            Subscriptions = new List<SubscriptionViewModel>().AsEnumerable();

            return this;
        }

    }


    #region SharedViewModel Helpers

    public struct SubscriptionViewModel
    {
        public string subName { get; set; }
        public string subURL { get; set; }
        public string subIcon { get; set; }
        
        /// <summary>
        /// The id of the entity that the user is subscribed too.
        /// </summary>
        public int subObjectId { get; set; }

        public SubscriptionViewModel Create(Subscription s)
        {
            if (s.TopicId != null)
            {
                subName = s.Topic.topicName;
                subURL = "/" + s.Topic.topicURL;
                subIcon = s.Topic.Image.icon_20;
                subObjectId = s.TopicId ?? 0;
            }
            else if (s.ProductId != null)
            {
                subName = s.Org.orgName + "-" + s.Product.ProductName;
                subURL = "/" + s.Org.orgURL + "/" + s.Product.ProductURL;
                subIcon = s.Product.Image.icon_20;
                subObjectId = s.ProductId ?? 0;
            }
            else
            {
                subName = s.Org.orgName;
                subURL = "/" + s.Org.orgURL;
                subIcon = s.Org.Image.icon_20;
                subObjectId = s.OrgId ?? 0;
            }

            return this;
        }
    }

    #endregion
}
