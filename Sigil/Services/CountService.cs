using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


namespace Sigil.Services
{
    public enum CountDataType
    {
        View, Vote, Subscription, Comment
    }

    //The operations we want to expose to the controllers;
    public interface ICountService
    {
        void CreateOrgCountData(int orgId);//ViewCount viewC, SubCount subC);
        void CreateIssueCountData(string userId, int orgId, int issueId);//ViewCount viewC, VoteCount voteC, CommentCount comC);
        void SaveOrgCountData();
        void SaveIssueCountData();

        void UpdateIssueViewCountData(Issue issue);
        void UpdateIssueVoteCountData(Issue issue, bool upVote = true);
        void SaveIssueDataChanges();

        /// <summary>
        /// Updated Orgs Subscription Data
        /// </summary>
        /// <param name="orgId">The orgs Id</param>
        /// <param name="subed">True(default) if adding a subscription, False if removing a subscription</param>
        void UpdateOrgSubscriptionCount(int orgId, bool subed = true);
        void SaveOrgDataChanges();

        void SaveCountChanges(CountCol count, CountDataType t, int orgId);
        void SaveCountChanges(CountCol count, CountDataType t, int orgid, int issueId);


        //I changed the order of the parameters half way through refactoring. Need to double check all calls to make sure parameters are in right order!!!!
        ViewCountCol GetIssueViewCount(int orgId, int issueId);
        VoteCountCol GetIssueVoteCount(int orgId, int issueId);
        CommentCountCol GetIssueCommentCount(int orgId, int issueId);

        IEnumerable<ViewCountCol> GetOrgViewCount(int orgId);
        IEnumerable<VoteCountCol> GetOrgVoteCount(int orgId);
        SubCountCol GetOrgSubscriptionCount(int orgId);
        IEnumerable<CommentCountCol> GetOrgCommentCount(int orgId);

        //UserVoteCol GetUserVotes(string userId);

    }

    public class CountService : ICountService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICountRepository countRespository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;




    }
}