﻿using System;
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

    enum NotificationType
    {
        Comment, OfficialResponse,
    }

    struct NotificationPanel
    {
        public string From { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
    }

    public class NotificationController : Controller
    {
        private readonly IUserService userService;
        private readonly INotificationService notificationService;
        private readonly IErrorService errorService;
        private readonly IIssueService issueService;

        public void Notification_Check(string text, string FromUser, int issueID, int orgID, int commentID)
        {
            string to_user = null;
           
            var text_words = text.Split(' ');
            foreach (string t in text_words)
            {
                if (t[0] == '@')
                {
                    to_user = t.Remove(0, 1);
                }
            }
            if (to_user != null) //|| to_org != null)
            {
                var ToUser = userService.GetUserByDisplayName(to_user);//dc.AspNetUsers.SingleOrDefault(u => u.DisplayName == to_user).Id;
                CreateNotification(ToUser.Id, FromUser, issueID, orgID, commentID, (int)NotificationType.Comment);
                notificationService.SaveNotification();
            }
        }

        /// <summary>
        /// Creates multiple notifications for everyone that has voted/commented on an issue that has recieved an Official Response
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issueID"></param>
        /// <param name="orgID"></param>
        /// <param name="commentID"></param>
        public void OfficialResponseNotificationRoutine(string userId, int issueId, int orgId, int commentID)
        {
            var VoteUsers = userService.GetUsersByVote(orgId, issueId);//dc.AspNetUsers.Select(u => u).ToList();//Where(u => CountXML<UserVoteCol>.XMLtoDATA(u.votes).Check_Vote(issueID, orgID) == true).Select(u => u);
            //List<AspNetUser> finalVoteUsers = new List<AspNetUser>();

            //foreach (var user in VoteUsers)
            //{
            //    if (CountXML<UserVoteCol>.XMLtoDATA(user.votes).Check_Vote(issueID, orgID))
            //    {
            //        finalVoteUsers.Add(user);
            //    }
            //}

            var CommentUsers = userService.GetUsersByIssue(orgId, issueId);//dc.Comments.Where(c => c.issueId == issueID && c.OrgId == orgID).Select(c => c.AspNetUser);

            var allUsers = VoteUsers.Union(CommentUsers);//finalVoteUsers.Union(CommentUsers).ToList();

            foreach(var user in allUsers)
            {
                CreateNotification(user.Id, userId, issueId, orgId, commentID, (int)NotificationType.OfficialResponse);
            }

            notificationService.SaveNotification();
        }

        /// <summary>
        /// Creates a notificiation table entry
        /// </summary>
        /// <param name="ToUserId"></param>
        /// <param name="FromUserId"></param>
        /// <param name="issueID"></param>
        /// <param name="orgID"></param>
        /// <param name="commentID"></param>
        /// <param name="Note_Type"></param>
        private void CreateNotification(string ToUserId, string FromUserId, int issueID, int orgID, int commentID, int Note_Type)
        {
            Notification note = new Notification();
            try
            {
                note.From_UserId = FromUserId;
                note.To_UserId = ToUserId;
                note.createTime = DateTime.UtcNow;
                note.issueId = issueID;
                note.OrgId = orgID;
                note.CommentId = commentID;
                note.NoteType = Note_Type;

                notificationService.CreateNotification(note);
                
                //we don't save the notification inside here just incase this function is being called by OfficialResponseNotification routine and multiply notifications are called.
            }
            catch (Exception e)
            {
                // ErrorHandler.Log_Error(note, e, dc);

                errorService.CreateError(note, e);
            }
        }

        public JsonResult Get_Notifications()
        {

            string userID = User.Identity.GetUserId();
            if(userID != null)
            {
                var notes = notificationService.GetUserNotifications(userID);//dc.Notifications.Where(n => n.To_UserId == userID).Select(n => n);
                if(!notes.Any())
                {
                    return Json("No Notifications");
                }
                
                List<NotificationPanel> rNotes = new List<NotificationPanel>();
                foreach(var n in notes)
                {
                    NotificationPanel tmp = new NotificationPanel();
                    var issue = issueService.GetIssue(n.OrgId, n.issueId);//dc.Issues.Single(i => i.Id == n.issueId && i.OrgId == n.OrgId);
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
                //ErrorHandler.Log_Error(userID, "No user id of when Get_Notifications was called.", dc);
                errorService.CreateError(userID, "No user id of when Get_Notifications was called.");
            }

            return Json("No notifications, you're all caught up. :)");
        }

    }
}