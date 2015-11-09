using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;
using System.Xml.Linq;

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
        IEnumerable<ApplicationUser> GetUsersByIssue(int orgId, int issueId);

        void CreateUserVote(string userId);
        void SaveUserVotes();

        void AddUserVote(ApplicationUser user, int orgId, int issueId);
        void AddUserVote(ApplicationUser user, int orgId, int issueId, int commentId);
        void RemoveUserVote(ApplicationUser user, int orgId, int issueId);
        void RemoveUserVote(ApplicationUser user, int orgId, int issueId, int commentId);
        void UpdateUser(ApplicationUser user);

        UserVoteCol GetUserVotes(string userId);
    }

    public class UserService : IUserService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        public UserService(IUnitOfWork unit, IUserRepository userRepo, ICommentRepository comRepo)
        {
            unitOfWork = unit;
            userRepository = userRepo;
            commentRespository = comRepo;
        }

        public ApplicationUser GetUser(string id)
        {
            return userRepository.GetById(id);
        }

        public ApplicationUser GetUserByDisplayName(string name)
        {
            return userRepository.GetByDisplayName(name);
        }

        public string GetUserDisplayName(string id)
        {
            return userRepository.GetDisplayName(id);
        }


        public IEnumerable<Comment> GetUserComments(string id)
        {
            return commentRespository.GetMany(c => c.UserId == id);
        }

        public IEnumerable<ApplicationUser> GetUsersByVote(int orgId, int issueId)
        {
            var users = userRepository.GetAll();

            List<ApplicationUser> votedUsers = new List<ApplicationUser>();
            foreach(var u in users)
            {
                if(CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(u.votes)).Check_Vote(issueId, orgId))
                {
                    votedUsers.Add(u);   
                }
            }

            return votedUsers.AsEnumerable();
        }

        public IEnumerable<ApplicationUser> GetUsersByIssue(int orgId, int issueId)
        {
            var issueComments = commentRespository.GetIssueComments(orgId, issueId);

            return issueComments.Select(c => c.User);
        }

        public void CreateUserVote(string userid)
        {
            var user = userRepository.GetById(userid);

            user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol()).ToString();
            userRepository.Update(user);
        }

        public void SaveUserVotes()
        {
            unitOfWork.Commit();
        }

        public void AddUserVote(ApplicationUser user, int orgId, int issueId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Add_Vote(issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void AddUserVote(ApplicationUser user, int orgId, int issueId, int commentId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Add_Vote(commentId, issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void RemoveUserVote(ApplicationUser user, int orgId, int issueId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Delete_Vote(issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void RemoveUserVote(ApplicationUser user, int orgId, int issueId, int commentId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(user.votes));
            userVoteCol.Delete_Vote(commentId, issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol).ToString();
            userRepository.Update(user);
        }

        public void UpdateUser(ApplicationUser user)
        {
            userRepository.Update(user);
        }

        public UserVoteCol GetUserVotes(string userId)
        {
            return CountXML<UserVoteCol>.XMLtoDATA(XElement.Parse(userRepository.GetById(userId).votes));
        }
    }
}