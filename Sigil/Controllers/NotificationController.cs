using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Sigil.Models;
using System.Threading.Tasks;
using Sigil.Services;
using Sigil.ViewModels;
namespace Sigil.Controllers
{
    public class NotificationController : Controller
    {
        //private readonly IUserService userService;
        private readonly INotificationService notificationService;
        private readonly IErrorService errorService;
        private readonly IIssueService issueService;
        private readonly IUserService userService;

       public NotificationController(INotificationService noteS, IIssueService issS, IErrorService errS, IUserService userS)
        {
            notificationService = noteS;
            issueService = issS;
            errorService = errS;
            userService = userS;
        }

        public class NoNotes {
            public string url = "";
            public string title = "No notifications, you're all caught up. :)";
        }

        public JsonResult Get_Notifications()
        {
            string userID = User.Identity.GetUserId();
            List<NotificationPanelVM> rNotes = new List<NotificationPanelVM>();
            var noNotes = new NotificationPanelVM();
            noNotes.URL = "";
            noNotes.Title = "No notifications, you're all caught up.";
            noNotes.Icon = "";
            Issue testiss = issueService.GetIssue(19);
            var testuser = userService.GetUser("68737fa9-6e72-4703-b46d-2096694715e7");
            if (userID != null)
            {
                var notes = notificationService.GetUserNotifications(userID).ToList();//dc.Notifications.Where(n => n.To_UserId == userID).Select(n => n);
                if (!notes.Any())
                {
                    rNotes.Add( noNotes );
                    return Json( rNotes.Select( n => new { from = n.From, title = n.Title, url = n.URL, icon=n.Icon } ), JsonRequestBehavior.AllowGet );
                }
                
                foreach (var n in notes)
                {
                    NotificationPanelVM tmp = new NotificationPanelVM();
                    var issue = issueService.GetIssue(n.issueId);//dc.Issues.Single(i => i.Id == n.issueId && i.OrgId == n.OrgId);
                    string from = n.From_UserId;
                    var fromUser = userService.GetUser(from);
                    tmp.From = fromUser.DisplayName;
                    tmp.Title = issue.title;
                    tmp.URL = issue.Product.Org.orgURL + "/" + issue.Product.ProductURL + "/" + issue.Id;
                    tmp.Icon = fromUser.Image.icon_100;
                    rNotes.Add(tmp);
                }

                return Json(rNotes.Select(n => new { from = n.From, title = n.Title, url = n.URL, icon = n.Icon}), JsonRequestBehavior.AllowGet);

            }
            else
            {
                //We have a serious problem haha
                //ErrorHandler.Log_Error(userID, "No user id of when Get_Notifications was called.", dc);
                errorService.CreateError(userID, "No user id of when Get_Notifications was called.");
            }
            
            rNotes.Add( noNotes );
            return Json( rNotes.Select( n => new { from = n.From, title = n.Title, url = n.URL } ), JsonRequestBehavior.AllowGet);
        }

    }
}