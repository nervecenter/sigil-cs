using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface ICountService
    {
        void CreateOrgCountData(ViewCount viewC, SubCount subC);
        void CreateIssueCountData(ViewCount viewC, VoteCount voteC, CommentCount comC);
        void SaveOrgCountData();
        void SaveIssueCouteData();
        //void UpdateCount(CountCol count);
        void SaveCountChanges(CountCol count, int orgid, int issueId);

        ViewCountCol GetIssueViewCount(int issueId, int orgId);
        VoteCountCol GetIssueVoteCount(int issueId, int orgId);
        SubCountCol GetIssueSubscriptionCount(int issueId, int orgId);
        CommentCountCol GetIssueCommentCount(int issueId, int orgId);

        IEnumerable<ViewCountCol> GetOrgViewCount(int orgId);
        IEnumerable<VoteCountCol> GetOrgVoteCount(int orgId);
        IEnumerable<SubCountCol> GetOrgSubscriptionCount(int orgId);
        IEnumerable<CommentCountCol> GetOrgCommentCount(int orgId);

        UserVoteCol GetUserVotes(string userId);

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