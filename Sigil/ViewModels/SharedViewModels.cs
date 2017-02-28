using Sigil.Models;
using Sigil.Controllers;
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
    
    public class IssuePanelPartialJsonVM
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

        public IssuePanelPartialJsonVM(Issue i, bool userVoted, bool inPanel) {
            issueID = i.Id;
            votes = i.votes;
            title = i.title;
            text = i.text;
            orgName = i.Product.Org.orgName;
            orgIcon = i.Product.Org.Image.icon_20;
            productName = i.Product.ProductName;
            productIcon = i.Product.Image.icon_20;
            datePostedString = i.createTime.ToShortDateString();
            userName = i.User.UserName;
            userIcon = i.User.Image.icon_20;
            UserVoted = userVoted;
            InPanel = inPanel;
        }

        public static string IssuePanelPartialHTML( IssuePanelPartialVM Model ) {
            string html = "<div class=\"panel " + ( Model.issue.responded ? "panel-info" : "panel-default") + " issue-panel-partial\"><div class=\"panel-body\"><div class=\"media\"><div class=\"media-object pull-left votebutton-box\">";
            //if( !Request.IsAuthenticated ) {
            html += "<img src=\"/Content/Images/notvoted.png\" class=\"voteup\" onclick=\"redirectToLogin()\" onmouseover=\"votehover(this)\" onmouseout=\"voteunhover(this)\">";
            /*} else if ( Model.UserVoted ) {
                html += "<img src=\"~/Content/Images/voted.png\" class=\"unvoteup\" onclick=\"unvoteup(this, " + Model.issue.Id + "\" onmouseover=\"votehover(this)\" onmouseout=\"voteunhover(this)\">";
            } else {
                html += "<img src=\"~/Content/Images/notvoted.png\" class=\"voteup\" onclick=\"voteup(this, " + Model.issue.Id + "\" onmouseover=\"votehover(this)\" onmouseout=\"voteunhover(this)\">";
            }*/
            html += "<br /><span id=\"count-" + Model.issue.Id + "\" class=\"voteamount\">" + Model.issue.votes + "</span></div><div class=\"media-body\"><h4 class=\"media-heading\"><a href = \"/" + Model.issue.Product.Org.orgURL + "/" + Model.issue.Product.ProductURL + "/" + Model.issue.Id + "\">" + Model.issue.title + "</a>";
            html += "</h4><p class=\"pull-left\"><img class=\"issue-panel-icon\" src=\"" + Model.issue.Product.Org.Image.icon_20 + "\"/><span><a href = \"" + Model.issue.Product.Org.orgURL + "\">" + Model.issue.Product.Org.orgName + "</a></span>";
            if ( Model.issue.Product.ProductURL != "Default" ) {
                html += "  <span class=\"label label-default\"><img class=\"issue-panel-icon\" src=\"" + Model.issue.Product.Image.icon_20 + "\" />" + Model.issue.Product.ProductName + "</span>";
            }
            if ( Model.issue.Product.TopicId != null ) {
                html += "<span>in <a href = \"/t/" + Model.issue.Product.Topic.topicURL + "\">" + Model.issue.Product.Topic.topicName + "</a></span>";
            }
            html += "</p><p class=\"pull-right\"><span>Posted by " + Model.issue.User.DisplayName + "</span></p></div></div></div>";

            if ( Model.issue.responded ) {
                var response = Model.issue.OfficialResponses[ 0 ].text;
                if ( response.Length > 100 ) {
                    response = response.Substring( 0, 85 ) + "...";
                }
                html += "<div class=\"panel-footer\"><b> Response: </b><span>" + response + "</span></div>";
            }
                
            html += "</div>";

            return html;
        }
    }


    struct NotificationPanelVM
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string Icon { get; set; }
    }


    public struct UserViewModel
    {
        public ApplicationUser User { get; set; }
        public UserVoteCol UserVotes { get; set; }
        public bool isOrgAdmin { get; set; }
        public string orgName { get; set; }
        public string orgURL { get; set; }
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

        //public IEnumerable<SubscriptionViewModel> Subscriptions { get; set; }

        public UserViewModel UserVM { get; set; }
        

        public SideBarVM Init()
        {
            showOrgBox = false;
            showSubscriptions = false;
            
            thisOrg = default(Org);

            orgProducts = new List<Product>().AsEnumerable();
            //Subscriptions = new List<SubscriptionViewModel>().AsEnumerable();
            UserVM = new UserViewModel().emptyUser();

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
                subURL = "/t/" + s.Topic.topicURL;
                subIcon = s.Topic.Image.icon_20;
                subObjectId = s.TopicId ?? 0;
            }
            else if (s.ProductId != null)
            {
                subName = (s.Product.ProductURL == "Default") ? s.Product.Org.orgName + "- General Feed" : s.Product.Org.orgName + "-" + s.Product.ProductName;
                subURL = "/" + s.Product.Org.orgURL + "/" + s.Product.ProductURL;
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
