using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    public enum CountType
    {
        VoteCount, ViewCount, SubCount, CommentCount
    }

    public interface IViewCountRepository : IRepository<ViewCount>
    {

        IEnumerable<ViewCount> GetOrgsViewCounts(int orgId);

        ViewCount GetIssueViewCount(int orgId, int issueId);

    }

    public interface IVoteCountRepository : IRepository<VoteCount>
    {
        IEnumerable<VoteCount> GetOrgsVoteCounts(int orgId);
        VoteCount GetIssueVoteCount(int orgId, int issueId);

        
    }

    public interface ISubscriptionCountRepository : IRepository<SubCount>
    {
        SubCount GetOrgsSubscriptionCount(int orgId);

        //void UpdateCount(CountType type, CountCol count, int orgId);

    }

    public interface ICommentCountRepository : IRepository<CommentCount>
    {
        IEnumerable<CommentCount> GetOrgsCommentCounts(int orgId);
        CommentCount GetIssueCommentCount(int orgId, int issueId);


       // void UpdateCount(CountType type, CountCol count, int orgId, int issueId);
    }

    public class ViewCountRepository : RepositoryBase<ViewCount>, IViewCountRepository
    {
        public ViewCountRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public ViewCount GetIssueViewCount(int orgId, int issueId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<ViewCount> IViewCountRepository.GetOrgsViewCounts(int orgId)
        {
            throw new NotImplementedException();
        }

        //where we define the Count methods created above

        IEnumerable<ViewCount> GetOrgsViewCounts(int orgId)
        {
            var counts = this.DbContext.ViewCountData.Where(vc => vc.OrgId == orgId).Select(vc => vc);
            return counts;
        }

    
    }

   
    public class VoteCountRepository : RepositoryBase<VoteCount>, IVoteCountRepository
    {
        public VoteCountRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }    

    public class SubscriptionCountRepository: RepositoryBase<SubCount>, ISubscriptionCountRepository
    {
        public SubscriptionCountRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }

    public class CommentCountRepository : RepositoryBase<CommentCount>, ICommentCountRepository
    {
        public CommentCountRepository(IDbFactory dbFactory) : base(dbFactory) { }

    }

}