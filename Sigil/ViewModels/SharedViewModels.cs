using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.ViewModels
{
    /// <summary>
    /// ViewModel for holding an Issue
    /// </summary>
    public class IssueViewModel {
        public int IssueId { get; set; }

        // need to replace category stuff with category vm
        public CategoryViewModel IssueCategoryVM { get; set; }

        public int NumVotes { get; set; }
        public bool userVoted { get; set; }
        public bool InPanel { get; set; }
        public bool Responded { get; set; }
        public string IssueUserDisplayName { get; set; }
        public Image IssueUserImage { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }

        public IssueViewModel( Issue i, bool Voted, bool inpanel) {
            IssueId = i.Id;

            IssueCategoryVM.CategoryId = i.CatId;
            IssueCategoryVM.CategoryName = i.Category.catName;
            IssueCategoryVM.CategoryURL = i.Category.catURL;
            IssueCategoryVM.CategoryImage = i.Category.Image;

            IssueCategoryVM.OrgVM.OrgId = i.Category.OrgId;
            IssueCategoryVM.OrgVM.OrgName = i.Category.Org.orgName;
            IssueCategoryVM.OrgVM.OrgURL = i.Category.Org.orgURL;
            IssueCategoryVM.OrgVM.OrgImage = i.Category.Org.Image;

            NumVotes = i.votes;
            userVoted = Voted;
            InPanel = inpanel;
            Responded = i.responded;
            IssueUserDisplayName = i.User.DisplayName;
            IssueUserImage = i.User.Image;
            Title = i.title;
            Text = i.text;
            CreateDate = i.createTime;
        }
    }

    /// <summary>
    /// ViewModel for holding a Comment
    /// </summary>
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int votes { get; set; }
        public bool userVoted { get; set; }
        public string CommentUserDisplayName { get; set; }
        public string CommentUserIcon { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }

        public CommentViewModel(Comment c, bool voted)
        {
            CommentId = c.Id;
            votes = c.votes;
            userVoted = voted;
            CommentUserDisplayName = c.User.DisplayName;
            CommentUserIcon = c.User.Image.icon_100;
            Text = c.text;
            CreateDate = c.createTime;
        }

    }

    /// <summary>
    /// ViewModel for holding a OfficialResponse
    /// </summary>
    public class OfficialResponseViewModel
    {
        public int OfficialResponseId { get; set; }

        public string OfficialUserDisplayName { get; set; }
        public string OfficialUserIcon { get; set; }

        public DateTime CreateTime { get; set; }
        //public DateTime editTime { get; set; }

        public string Text { get; set; }

        public int upVotes { get; set; }
        public int downVotes { get; set; }


        public OfficialResponseViewModel(OfficialResponse o)
        {
            OfficialResponseId = o.Id;
            OfficialUserDisplayName = o.User.DisplayName;
            OfficialUserIcon = o.User.Image.icon_100;
            CreateTime = o.createTime;
            Text = o.text;
            upVotes = o.upVotes;
            downVotes = o.downVotes;
        }
    }

    

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryURL { get; set; }
        public Image CategoryImage { get; set; }

        public OrgViewModel OrgVM { get; set; }
    }


    /// <summary>
    /// ViewModel for holding and Org
    /// </summary>
    public class OrgViewModel
    {
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgURL { get; set; }
        public Image OrgImage { get; set; }

        public OrgViewModel(Org o)
        {
            OrgId = o.Id;
            OrgName = o.orgName;
            OrgURL = o.orgURL;
            OrgImage = o.Image;
        }

    }

    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public UserVoteCol UserVotes { get; set; }

        public IEnumerable<Notification> UserNotifications { get; set; }
        public IEnumerable<SubscriptionViewModel> UserSubscriptions { get; set; }
    }


    #region UserViewModel Helpers

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

    #endregion
}
