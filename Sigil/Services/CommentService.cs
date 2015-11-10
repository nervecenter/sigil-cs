using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;
using System.Threading;
using System.Xml.Linq;

namespace Sigil.Services
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

    //The operations we want to expose to the controllers
    public interface ICommentService
    {
        void CreateComment(Comment comment);
        void EditComment(Comment comment, string newText);
        void SaveComment();
        void DeleteComment(Comment comment);

        Comment GetComment(int orgId, int issueId, int commentId);

        IEnumerable<OfficialResponse> GetIssuesOfficialResponses(int orgId, int issueId);
        IEnumerable<OfficialResponse> GetOrgsOfficialResponses(int orgId);
        OfficialResponse GetIssueLatestOfficialResponse(int orgId, int issueId);

        void Comment_POST_Handler(HttpRequestBase request, Issue thisIssue, string userID);

        IEnumerable<Comment> GetUserComments(string userId);
        IEnumerable<Comment> GetIssueComments(int orgId, int issueId);
        IEnumerable<Comment> GetOrgComments(int orgId);

    }

    public class CommentService : ICommentService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICommentRepository commentRespository;
        private readonly ICommentCountRepository commentCountRepository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly INotificationRepository notificationRepository;
        private readonly IOfficialResponseRepository officialResponseRepository;
        private readonly IUserService userService;
        
        //private readonly ICountService countDataService;

        public CommentService(IUnitOfWork unit, ICommentRepository commRepo, IOfficialResponseRepository offRepo, ICommentCountRepository comCRepo, IUserRepository userRepo)
        {
            this.unitOfWork = unit;
            this.commentRespository = commRepo;
            this.officialResponseRepository = offRepo;
            commentCountRepository = comCRepo;
            userRepository = userRepo;
        }

        public void CreateComment(Comment comm)
        {
            commentRespository.Add(comm);
        }

        public void EditComment(Comment comm, string newText)
        {
            comm.text = newText;
            commentRespository.Update(comm);
            SaveComment();
        }

        public void SaveComment()
        {
            unitOfWork.Commit();
        }

        public void DeleteComment(Comment comm)
        {
            commentRespository.Delete(comm);
            SaveComment();
        }

        public Comment GetComment(int orgId, int issueId, int commentId)
        {
            return commentRespository.GetById(orgId, issueId, commentId);
        }

        public IEnumerable<OfficialResponse> GetIssuesOfficialResponses(int orgId, int issueId)
        {
            return officialResponseRepository.GetMany(o => o.Issue.Category.OrgId == orgId && o.issueId == issueId);
        }

        public IEnumerable<OfficialResponse> GetOrgsOfficialResponses(int orgId)
        {
            return officialResponseRepository.GetMany(o => o.Issue.Category.OrgId == orgId);
        }

        public OfficialResponse GetIssueLatestOfficialResponse(int orgId, int issueId)
        {
            return officialResponseRepository.GetMany(o => o.Issue.Category.OrgId == orgId && o.issueId == issueId).OrderByDescending(o => o.createTime).FirstOrDefault();
        }

        public IEnumerable<Comment> GetUserComments(string userId)
        {
            return commentRespository.GetMany(c => c.UserId == userId);
        }

        public IEnumerable<Comment> GetIssueComments(int orgId, int issueId)
        {
            return commentRespository.GetMany(c => c.Issue.Category.OrgId == orgId && c.IssueId == issueId);
        }

        public IEnumerable<Comment> GetOrgComments(int orgId)
        {
            return commentRespository.GetMany(c => c.Issue.Category.OrgId == orgId);
        }

        public void Comment_POST_Handler(HttpRequestBase request, Issue thisIssue, string userID)
        {
            if (request.Form["IsOfficial"] != null)
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
        private void Create_New_Comment(HttpRequestBase request, Issue thisIssue, string userID)
        {
            Comment newComment = new Comment();

            // Increment Id, drop in current user and date, set default weight, drop in the form text
            newComment.IssueId = thisIssue.Id;
            newComment.UserId = userID;
            newComment.createTime = DateTime.UtcNow;
            newComment.editTime = DateTime.UtcNow;
            newComment.lastVoted = DateTime.UtcNow;
            newComment.votes = 1;
            
            newComment.text = request.Form["text"];

            Thread CommCountThread = new Thread(() => CommentCountRoutine(thisIssue.Category.OrgId, thisIssue.Id));
            CommCountThread.Start();

            CreateComment(newComment);
            SaveComment();

           

            Thread NotificationThread = new Thread(() => Notification_Check(newComment.text, userID, thisIssue.Id, thisIssue.Category.OrgId, newComment.Id));
            NotificationThread.Start();
        }

        private void Create_Official_Response(HttpRequestBase request, Issue thisIssue, string userID)
        {
            OfficialResponse newOff = new OfficialResponse();

            newOff.createTime = DateTime.UtcNow;
            newOff.downVotes = 0;
            newOff.upVotes = 1;
            newOff.issueId = thisIssue.Id;
            //newOff.OrgId = thisIssue.OrgId;
            newOff.text = request.Form["text"];
            newOff.UserId = userID;


            officialResponseRepository.Add(newOff);
            

            //Checking to see of the official response mentions any users specifically
            Thread NotificationThread = new Thread(() => Notification_Check(newOff.text, userID, thisIssue.Id, thisIssue.Category.OrgId, newOff.Id));
            NotificationThread.Start();

            //Notifies every user who has commented and or voted on the issue that an official response has been made
            Thread NotificationThread2 = new Thread(() => OfficialResponseNotificationRoutine(userID, thisIssue.Id, thisIssue.Category.OrgId, newOff.Id));
            NotificationThread2.Start();

        }

        private void CommentCountRoutine(int orgId, int issueId)
        {

            //CommentCountCol comCol;


            var commentData = commentCountRepository.GetIssueCommentCount(orgId, issueId);//countDataService.GetIssueCommentCountCol(issueId, orgId);

            var commentCol = CountXML<CommentCountCol>.XMLtoDATA(XElement.Parse(commentData.count));
            commentCol.Update();
            commentData.count = CountXML<CommentCountCol>.DATAtoXML(commentCol).ToString();

            commentCountRepository.Update(commentData);

        }

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
                var ToUser = userRepository.GetByDisplayName(to_user);//dc.AspNetUsers.SingleOrDefault(u => u.DisplayName == to_user).Id;
                CreateNotification(ToUser.Id, FromUser, issueID, orgID, commentID, (int)NotificationType.Comment);
                unitOfWork.Commit();
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
            var VoteUsers = userService.GetUsersByVote(orgId, issueId);

            var CommentUsers = userService.GetUsersByIssue(orgId, issueId);

            var allUsers = VoteUsers.Union(CommentUsers);

            foreach (var user in allUsers)
            {
                CreateNotification(user.Id, userId, issueId, orgId, commentID, (int)NotificationType.OfficialResponse);
            }

            unitOfWork.Commit();
            //notificationService.SaveNotification();
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
                note.orgId = orgID;
                note.CommentId = commentID;
                note.NoteType = Note_Type;

                notificationRepository.Add(note);
                //notificationService.CreateNotification(note);

                //we don't save the notification inside here just incase this function is being called by OfficialResponseNotification routine and multiply notifications are called.
            }
            catch (Exception e)
            {
                // ErrorHandler.Log_Error(note, e, dc);

                //errorService.CreateError(note, e);
            }
        }

    }
}