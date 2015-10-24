using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sigil.Models;

namespace Sigil.Controllers
{
    public class CommentController : Controller
    {
        private static SigilDBDataContext dc = new SigilDBDataContext();

        /// <summary>
        /// Creates a new comment and saves to DB
        /// </summary>
        /// <param name="thisIssue">Issue the comment is for</param>
        /// <param name="userID">Comment creators user ID</param>
        public static void Create_New_Comment(HttpRequestBase request, Issue thisIssue, string userID)
        {
            Comment newComment = new Comment();

            // Increment Id, drop in current user and date, set default weight, drop in the form text
            newComment.issueId = thisIssue.Id;
            newComment.UserId = userID;
            newComment.postDate = DateTime.UtcNow;
            newComment.editTime = DateTime.UtcNow;
            newComment.lastVoted = DateTime.UtcNow;
            newComment.votes = 1;

            newComment.text = request.Form["text"];
            NotificationController.Notification_Check(newComment.text, userID);

            var commentData = dc.CommentCounts.SingleOrDefault(c => c.OrgId == thisIssue.OrgId && c.IssueId == thisIssue.Id);
            if (commentData == default(CommentCount))
            {
                CommentCount newCount = new CommentCount();
                CommentCountCol comCol = new CommentCountCol();
                comCol.Update();
                newCount.count = CountXML<CommentCountCol>.DATAtoXML(comCol);
                newCount.IssueId = thisIssue.Id;
                newCount.OrgId = thisIssue.OrgId;
                try
                {
                    dc.CommentCounts.InsertOnSubmit(newCount);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    ErrorHandler.Log_Error(newCount, e, dc);
                }
            }
            else
            {

                var commDataCol = CountXML<CommentCountCol>.XMLtoDATA(commentData.count);
                commDataCol.Update();
                commentData.count = CountXML<CommentCountCol>.DATAtoXML(commDataCol);
            }
            // Try to submit the issue and go to the issue page; otherwise, write an error
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
        }
    }
}