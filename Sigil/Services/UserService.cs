using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IUserService
    {
        AspNetUser GetUser(string id);
        AspNetUser GetUserByDisplayName(string name);
        string GetUserDisplayName(string id);
        void SetUserRole(AspNetRole role, string id);

        IEnumerable<Comment> GetUserComments(string id);

        /// <summary>
        /// Returns list of users who voted on an issue.
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IEnumerable<AspNetUser> GetUsersByVote(int orgId, int issueId);

        /// <summary>
        /// Returns list of users who commented on an issue.
        /// </summary>
        /// <param name="orgId"> Org Id of the issue.</param>
        /// <param name="issueId"> Issue Id</param>
        /// <returns></returns>
        IEnumerable<AspNetUser> GetUsersByIssue(int orgId, int issueId);

        void CreateUserVote(AspNetUser user);
        void SaveUserVotes();

        void AddUserVote(AspNetUser user, int orgId, int issueId);
        void AddUserVote(AspNetUser user, int orgId, int issueId, int commentId);
        void RemoveUserVote(AspNetUser user, int orgId, int issueId);
        void RemoveUserVote(AspNetUser user, int orgId, int issueId, int commentId);
        void UpdateUser(AspNetUser user);

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

        public AspNetUser GetUser(string id)
        {
            return userRepository.GetById(id);
        }

        public AspNetUser GetUserByDisplayName(string name)
        {
            return userRepository.GetByDisplayName(name);
        }

        public string GetUserDisplayName(string id)
        {
            return userRepository.GetDisplayName(id);
        }

        public void SetUserRole(AspNetRole role, string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Comment> GetUserComments(string id)
        {
            return commentRespository.GetMany(c => c.UserId == id);
        }

        public IEnumerable<AspNetUser> GetUsersByVote(int orgId, int issueId)
        {
            var users = userRepository.GetAll();

            List<AspNetUser> votedUsers = new List<AspNetUser>();
            foreach(var u in users)
            {
                if(CountXML<UserVoteCol>.XMLtoDATA(u.votes).Check_Vote(issueId, orgId))
                {
                    votedUsers.Add(u);   
                }
            }

            return votedUsers.AsEnumerable();
        }

        public IEnumerable<AspNetUser> GetUsersByIssue(int orgId, int issueId)
        {
            var issueComments = commentRespository.GetIssueComments(orgId, issueId);

            return issueComments.Select(c => c.AspNetUser);
        }

        public void CreateUserVote(AspNetUser user)
        {
            user.votes = CountXML<UserVoteCol>.DATAtoXML(new UserVoteCol());
            userRepository.Update(user);
        }

        public void SaveUserVotes()
        {
            unitOfWork.Commit();
        }

        public void AddUserVote(AspNetUser user, int orgId, int issueId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(user.votes);
            userVoteCol.Add_Vote(issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol);
            userRepository.Update(user);
        }

        public void AddUserVote(AspNetUser user, int orgId, int issueId, int commentId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(user.votes);
            userVoteCol.Add_Vote(commentId, issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol);
            userRepository.Update(user);
        }

        public void RemoveUserVote(AspNetUser user, int orgId, int issueId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(user.votes);
            userVoteCol.Delete_Vote(issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol);
            userRepository.Update(user);
        }

        public void RemoveUserVote(AspNetUser user, int orgId, int issueId, int commentId)
        {
            var userVoteCol = CountXML<UserVoteCol>.XMLtoDATA(user.votes);
            userVoteCol.Delete_Vote(commentId, issueId, orgId);
            user.votes = CountXML<UserVoteCol>.DATAtoXML(userVoteCol);
            userRepository.Update(user);
        }

        public void UpdateUser(AspNetUser user)
        {
            userRepository.Update(user);
        }

        public UserVoteCol GetUserVotes(string userId)
        {
            return CountXML<UserVoteCol>.XMLtoDATA(userRepository.GetById(userId).votes);
        }
    }
}