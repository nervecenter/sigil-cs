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

        ViewCount GetOrgsViewCount(int orgid);

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

        //where we define the Count methods created above

        public ViewCount GetIssueViewCount(int orgId, int issueId)
        {
            var vc = this.DbContext.ViewCountData.Where(v => v.OrgId == orgId && v.IssueId == issueId).FirstOrDefault();
            return vc;
        }

        public ViewCount GetOrgsViewCount(int orgid)
        {
            var vc = this.DbContext.ViewCountData.Where(v => v.OrgId == orgid && v.IssueId == 0).FirstOrDefault();
            return vc;
        }

        IEnumerable<ViewCount> IViewCountRepository.GetOrgsViewCounts(int orgId)
        {
            var vc = this.DbContext.ViewCountData.Where(v => v.OrgId == orgId).Select(v => v);
            return vc;
        }

        

        IEnumerable<ViewCount> GetOrgsViewCounts(int orgId)
        {
            var counts = this.DbContext.ViewCountData.Where(vc => vc.OrgId == orgId).Select(vc => vc);
            return counts;
        }

    
    }

   
    public class VoteCountRepository : RepositoryBase<VoteCount>, IVoteCountRepository
    {
        public VoteCountRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public VoteCount GetIssueVoteCount(int orgId, int issueId)
        {
            var vc = this.DbContext.VoteCountData.Where(v => v.OrgId == orgId && v.IssueId == issueId).FirstOrDefault();
            return vc;
        }

        public IEnumerable<VoteCount> GetOrgsVoteCounts(int orgId)
        {
            var vc = this.DbContext.VoteCountData.Where(v => v.OrgId == orgId).Select(v => v);
            return vc;
        }
    }    

    public class SubscriptionCountRepository: RepositoryBase<SubCount>, ISubscriptionCountRepository
    {
        public SubscriptionCountRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public SubCount GetOrgsSubscriptionCount(int orgId)
        {
            var vc = this.DbContext.SubscriptionCountData.Where(v => v.OrgId == orgId).FirstOrDefault();
            return vc;
        }
    }

    public class CommentCountRepository : RepositoryBase<CommentCount>, ICommentCountRepository
    {
        public CommentCountRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public CommentCount GetIssueCommentCount(int orgId, int issueId)
        {
            var vc = this.DbContext.CommentCountData.Where(v => v.OrgId == orgId && v.IssueId == issueId).FirstOrDefault();
            return vc;
        }

        public IEnumerable<CommentCount> GetOrgsCommentCounts(int orgId)
        {
            var vc = this.DbContext.CommentCountData.Where(v => v.OrgId == orgId).Select(v => v);
            return vc;
        }
    }

}