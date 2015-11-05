using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


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
        private readonly ICountRepository countRespository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;

        public UserService(IUserRepository userRepo, )


    }
}