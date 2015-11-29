using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Sigil.Models;
using System.Threading.Tasks;
using Sigil.Services;

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

        public JsonResult Get_Notifications()
        {

            string userID = User.Identity.GetUserId();
            if (userID != null)
            {
                var notes = notificationService.GetUserNotifications(userID);//dc.Notifications.Where(n => n.To_UserId == userID).Select(n => n);
                if (!notes.Any())
                {
                    return Json("No Notifications");
                }

                List<NotificationPanel> rNotes = new List<NotificationPanel>();
                foreach (var n in notes)
                {
                    NotificationPanel tmp = new NotificationPanel();
                    var issue = issueService.GetIssue(n.orgId, n.productId, n.issueId);//dc.Issues.Single(i => i.Id == n.issueId && i.OrgId == n.OrgId);
                    tmp.From = n.From_UserId;
                    tmp.Title = issue.title;
                    tmp.URL = issue.Product.Org.orgName + "/" + issue.Product.ProductURL + "/" + issue.Id;
                    rNotes.Add(tmp);
                }

                return Json(rNotes.Select(n => new { from = n.From, title = n.Title, url = n.URL }), JsonRequestBehavior.AllowGet);

            }
            else
            {
                //We have a serious problem haha
                //ErrorHandler.Log_Error(userID, "No user id of when Get_Notifications was called.", dc);
                errorService.CreateError(userID, "No user id of when Get_Notifications was called.");
            }

            return Json("No notifications, you're all caught up. :)");
        }

    }
}