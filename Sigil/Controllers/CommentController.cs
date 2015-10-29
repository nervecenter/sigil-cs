using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;
using System.Threading;

namespace Sigil.Controllers
{
    public class CommentController : Controller
    {
        private SigilDBDataContext dc;


        public CommentController()
        {
            dc = new SigilDBDataContext();
        }

        public void Comment_Handler(HttpRequestBase request, Issue thisIssue, string userID)
        {
            if(request.Form["IsOfficial"] != null)
            {
                Create_Official_Response(request, thisIssue, userID);
            }
            else
            {
                Create_New_Comment(request, thisIssue, userID);
            }
        }

        /// <summary>
        /// Creates a new comment and saves to DB
        /// </summary>
        /// <param name="thisIssue">Issue the comment is for</param>
        /// <param name="userID">Comment creators user ID</param>
        public void Create_New_Comment(HttpRequestBase request, Issue thisIssue, string userID)
        {
            Comment newComment = new Comment();

            // Increment Id, drop in current user and date, set default weight, drop in the form text
            newComment.issueId = thisIssue.Id;
            newComment.UserId = userID;
            newComment.postDate = DateTime.UtcNow;
            newComment.editTime = DateTime.UtcNow;
            newComment.lastVoted = DateTime.UtcNow;
            newComment.votes = 1;
            newComment.IsOfficialResponse = false;
            newComment.text = request.Form["text"];

            Thread CommCountThread = new Thread(() => CommentCountRoutine(thisIssue.OrgId, thisIssue.Id));
            CommCountThread.Start();

            try
            {
                dc.Comments.InsertOnSubmit(newComment);

                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                //WRITE TO ERROR FILE
                ErrorHandler.Log_Error(newComment, e, dc);
                //ErrorHandler.Log_Error(commentData, e);

            }

            Thread NotificationThread = new Thread(() => NotificationController.Notification_Check(newComment.text, userID, thisIssue.Id, thisIssue.OrgId, newComment.Id));
            NotificationThread.Start();
        }

        public void Create_Official_Response(HttpRequestBase request, Issue thisIssue, string userID)
        {
            OfficialResponse newOff = new OfficialResponse();

            newOff.createTime = DateTime.UtcNow;
            newOff.downVotes = 0;
            newOff.upVotes = 1;
            newOff.issueId = thisIssue.Id;
            newOff.OrgId = thisIssue.OrgId;
            newOff.text = request.Form["text"];
            newOff.UserId = userID;

            try
            {
                dc.OfficialResponses.InsertOnSubmit(newOff);

                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                //WRITE TO ERROR FILE
                ErrorHandler.Log_Error(newOff, e, dc);
                //ErrorHandler.Log_Error(commentData, e);

            }

            //Checking to see of the official response mentions any users specifically
            Thread NotificationThread = new Thread(() => NotificationController.Notification_Check(newOff.text, userID, thisIssue.Id, thisIssue.OrgId, newOff.Id));
            NotificationThread.Start();

            //Notifies every user who has commented and or voted on the issue that an official response has been made
            Thread NotificationThread2 = new Thread(() => NotificationController.OfficialResponseNotificationRoutine(userID, thisIssue.Id, thisIssue.OrgId, newOff.Id));
            NotificationThread2.Start();
        }

        private void CommentCountRoutine(int orgId, int issueId)
        {

            CommentCountCol comCol;
            var commentData = dc.CommentCounts.SingleOrDefault(c => c.OrgId == orgId && c.IssueId == issueId);
            if (commentData == default(CommentCount))
            {
                CommentCount newCount = new CommentCount();
                comCol = new CommentCountCol();
                comCol.Update();
                newCount.count = CountXML<CommentCountCol>.DATAtoXML(comCol);
                newCount.IssueId = issueId;
                newCount.OrgId = orgId;
                try
                {
                    dc.CommentCounts.InsertOnSubmit(newCount);
                }
                catch (Exception e)
                {
                    ErrorHandler.Log_Error(newCount, e, dc);
                }
            }
            else
            {

                comCol = CountXML<CommentCountCol>.XMLtoDATA(commentData.count);
                comCol.Update();
                commentData.count = CountXML<CommentCountCol>.DATAtoXML(comCol);
            }

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorHandler.Log_Error(comCol, e, dc);
            }
        }
    }
}