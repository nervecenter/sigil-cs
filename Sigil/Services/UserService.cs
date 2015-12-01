using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.ViewModels;
using Sigil.Repository;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IUserService
    {
        ApplicationUser GetUser(string id);
        ApplicationUser GetUserByDisplayName(string name);
        string GetUserDisplayName(string id);
        //void SetUserRole(AspNetRole role, string id);

        IEnumerable<Comment> GetUserComments(string id);

        /// <summary>
        /// Returns list of users who voted on an issue.
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetUsersByVote(int orgId, int issueId);

        /// <summary>
        /// Returns list of users who commented on an issue.
        /// </summary>
        /// <param name="orgId"> Org Id of the issue.</param>
        /// <param name="issueId"> Issue Id</param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetUsersByIssue(int issueId);

        void AssignUserImage(string userId, Image img);

        void CreateUserVote(string userId);
        void SaveUserVotes();

        void AddUserVote(ApplicationUser user, int issueId);
        void AddUserVote(ApplicationUser user, int issueId, int commentId);
        void RemoveUserVote(ApplicationUser user, int issueId);
        void RemoveUserVote(ApplicationUser user, int issueId, int commentId);
        void UpdateUser(ApplicationUser user);

        UserViewModel GetUserViewModel(string userId);

        UserVoteCol GetUserVotes(string userId);

        //IdentityRole functions
        void CreateRole(string roleName);
        IEnumerable<IdentityRole> GetAllRoles();
        void DeleteRole(string roleName);
        void EditRole(string roleName, string newRoleName);
        //void AssignUserToRole(string userId, string RoleName);
        //IEnumerable<IdentityRole> GetUserRoles(string userId);
        //void RemoveUserRole(string userId, string roleName);
    }

    public class UserService : IUserService
    {
        //private readonly IOrgRepository OrgsRepository;
        //private readonly IProductRepository categoryRepository;
        //private readonly IIssueRepository issueRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRepository;
        
        private readonly IUnitOfWork unitOfWork;

        private readonly IErrorService errorService;

        public UserService(IUnitOfWork unit, IUserRepository userRepo, ICommentRepository comRepo, INotificationRepository noteRepo, ISubscriptionRepository subRepo, IErrorService errS)
        {
            unitOfWork = unit;
            userRepository = userRepo;
            commentRespository = comRepo;
            notificationRepository = noteRepo;
            subscriptionRepository = subRepo;
            errorService = errS;
        }

        public ApplicationUser GetUser(string id)
        {
            var user = userRepository.GetById(id);
            if(user == null)
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(id, "User not found by id./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Critical);
            }

            return user ?? default(ApplicationUser);
        }

        public ApplicationUser GetUserByDisplayName(string name)
        {
            var user = userRepository.GetByDisplayName(name);
            if (user == null)
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(name, "User not found by displayname./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Critical);
            }

            return user ?? default(ApplicationUser);
        }

        public string GetUserDisplayName(string id)
        {
            var user = userRepository.GetDisplayName(id);
            if (user == null)
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(id, "User not found by id./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Critical);
            }

            return user ?? "";
        }


        public IEnumerable<Comment> GetUserComments(string id)
        {
            return commentRespository.GetMany(c => c.UserId == id);
        }

        public IEnumerable<ApplicationUser> GetUsersByVote(int orgId, int issueId)
        {
            var users = userRepository.GetAll();
            List<ApplicationUser> votedUsers = new List<ApplicationUser>();

            if (users.Count() == 0)
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(issueId, "No users voted on issue./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Warning);
                return votedUsers;
            }
            foreach(var u in users)
            {
                if(CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(u.votes)).Check_Vote(issueId, orgId))
                {
                    votedUsers.Add(u);   
                }
            }

            return votedUsers.AsEnumerable();
        }

        public IEnumerable<ApplicationUser> GetUsersByIssue(int issueId)
        {
            var issueComments = commentRespository.GetIssueComments(issueId);

            return issueComments.Select(c => c.User) ?? new List<ApplicationUser>().AsEnumerable();
        }

        public void CreateUserVote(string userid)
        {
            var user = GetUser(userid);
            if (user == default(ApplicationUser))
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(userid, "User not found by id. Could not create UserVote./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Critical);
                return;
            }
            user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol()).ToString();
            userRepository.Update(user);
        }

        public void SaveUserVotes()
        {
            unitOfWork.Commit();
        }

        public void AddUserVote(ApplicationUser user, int issueId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Add_Vote(issueId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void AddUserVote(ApplicationUser user, int issueId, int commentId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Add_Vote(issueId, commentId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void RemoveUserVote(ApplicationUser user, int issueId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Delete_Vote(issueId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void RemoveUserVote(ApplicationUser user, int issueId, int commentId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Delete_Vote(issueId, commentId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void UpdateUser(ApplicationUser user)
        {
            userRepository.Update(user);
        }

        public UserVoteCol GetUserVotes(string userId)
        {
            var user = userRepository.GetById(userId);
            if (user == default(ApplicationUser))
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(userId, "User not found by id. Could not retrieve user vote col./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Critical);
                return new UserVoteCol();
            }
            if(user.votes == null) //should be able to remove this if check once we are sure the database seed data is making it correctly.
            {
                user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol()).ToString();
                UpdateUser(user);
            }
            return CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
        }

        public UserViewModel GetUserViewModel(string userId)
        {
            UserViewModel userVM = new UserViewModel();
            userVM.User = GetUser(userId);
            
            userVM.UserNotifications = notificationRepository.GetUsersNotifications(userId);
            userVM.UserSubscriptions = subscriptionRepository.GetMany(s => s.UserId == userId).Select(s => new SubscriptionViewModel().Create(s));
            userVM.UserVotes = GetUserVotes(userId);

            return userVM;
        }

        public void AssignUserImage(string userId, Image img)
        {
            var user = GetUser(userId);
            user.Image = img;
            user.ImageId = img.Id;
            userRepository.Update(user);
            unitOfWork.Commit();
        }

        public void CreateRole(string roleName)
        {
            IdentityRole newRole = new IdentityRole(roleName);
            userRepository.CreateRole(newRole);
            unitOfWork.Commit();
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return userRepository.GetAllRoles();
        }

        public void DeleteRole(string roleName)
        {
            userRepository.DeleteRole(roleName);
            unitOfWork.Commit();
        }

        public void EditRole(string roleName, string newRoleName)
        {
            userRepository.EditRole(roleName, newRoleName);
            unitOfWork.Commit();
        }

        //public void AssignUserToRole(string userId, string RoleName)
        //{
        //    userRepository.AssignUserToRole(userId, RoleName);
        //    unitOfWork.Commit();
        //}

        //public IEnumerable<IdentityRole> GetUserRoles(string userId)
        //{
        //    return userRepository.GetUserRoles(userId);
        //}

        //public void RemoveUserRole(string userId, string roleName)
        //{
        //    userRepository.RemoveUserRole(userId, roleName);
        //    unitOfWork.Commit();
        //}
    }
}