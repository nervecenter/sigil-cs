using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Sigil.Models;

namespace Sigil.Controllers
{
    struct NotificationPanel
    {
        public string From { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
    }

    public class NotificationController : Controller
    {
        private static SigilDBDataContext dc = new SigilDBDataContext();

        public static void Notification_Check(string text, string user, int issueID, int orgID, int commentID)
        {
            string to_user = null;
            string to_org = null;
            var text_words = text.Split(' ');
            foreach (string t in text_words)
            {
                if (t[0] == '@')
                {
                    to_user = t.Remove(0, 1);

                }

            }

            if (to_user != null || to_org != null)
            {
                var to_user_id = from users in dc.AspNetUsers
                                 where users.DisplayName == to_user
                                 select users.Id;

                var to_org_id = from org in dc.Orgs
                                where org.orgURL == to_org
                                select org.Id;

                Notification note = new Notification();
                try
                {
                    note.From_UserId = user;
                    note.To_UserId = to_user_id.Cast<string>().First();
                    //note.To_OrgId = Convert.ToInt32(to_org_id.ToString());
                    note.createTime = DateTime.UtcNow;
                    note.issueId = issueID;
                    note.OrgId = orgID;
                    note.CommentId = commentID;
                    dc.Notifications.InsertOnSubmit(note);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    ErrorHandler.Log_Error(note, e, dc);
                    //Console.WriteLine("Could not unvote on issue %s:\n%s", note.Id, e.Message);
                }
            }
        }

        public JsonResult Get_Notifications()
        {

            string userID = User.Identity.GetUserId();
            if(userID != null)
            {
                var notes = dc.Notifications.Where(n => n.To_UserId == userID).Select(n => n);
                if(!notes.Any())
                {
                    return Json("No Notifications");
                }
                
                List<NotificationPanel> rNotes = new List<NotificationPanel>();
                foreach(var n in notes)
                {
                    NotificationPanel tmp = new NotificationPanel();
                    var issue = dc.Issues.Single(i => i.Id == n.issueId && i.OrgId == n.OrgId);
                    tmp.From = n.From_UserId;
                    tmp.Title = issue.title;
                    tmp.URL = issue.Org.orgName + "/" + issue.Id;
                    rNotes.Add(tmp);
                }

                return Json(rNotes.Select(n => new { from = n.From, title = n.Title, url = n.URL }), JsonRequestBehavior.AllowGet);

            }
            else
            {
                //We have a serious problem haha
                ErrorHandler.Log_Error(userID, "No user id of when Get_Notifications was called.", dc);
                
            }

            return Json("No notifications, you're all caught up. :)");
        }

    }
}