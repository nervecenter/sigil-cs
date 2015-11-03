using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public enum CountType
    {
        VoteCount, ViewCount, SubCount, CommentCount
    }

    public interface ICountRepository : IRepository<Count>
    {

        IEnumerable<ViewCountCol> GetOrgsViewCounts(int orgId);
        IEnumerable<VoteCountCol> GetOrgsVoteCounts(int orgId);
        IEnumerable<SubCountCol> GetOrgsSubscriptionCounts(int orgId);
        IEnumerable<CommentCountCol> GetOrgsCommentCounts(int orgId);

        ViewCountCol GetIssueViewCount(int orgId, int issueId);
        VoteCountCol GetIssueVoteCount(int orgId, int issueId);
        CommentCountCol GetIssueCommentCount(int orgId, int issueId);

        void UpdateCount(CountType type, CountCol count, int orgId);
        void UpdateCount(CountType type, CountCol count, int orgId, int issueId);

    }

    public class CountRepository : RepositoryBase<Count>, ICountRepository
    {
        public CountRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Count methods created above

        IEnumerable<ViewCountCol> GetOrgsViewCounts(int orgId)
        {
            var counts = this.DbContext.ViewCountData.Where(vc => vc.OrgId == orgId).Select(vc => CountXML<ViewCountCol>.XMLtoDATA(vc.count));
            return counts;
        }

        IEnumerable<ViewCountCol> GetOrgsViewCounts(int orgId)
        {
            var counts = this.DbContext.ViewCountData.Where(vc => vc.OrgId == orgId).Select(vc => CountXML<ViewCountCol>.XMLtoDATA(vc.count));
            return counts;
        }

        IEnumerable<ViewCountCol> GetOrgsViewCounts(int orgId)
        {
            var counts = this.DbContext.ViewCountData.Where(vc => vc.OrgId == orgId).Select(vc => CountXML<ViewCountCol>.XMLtoDATA(vc.count));
            return counts;
        }

        IEnumerable<ViewCountCol> GetOrgsViewCounts(int orgId)
        {
            var counts = this.DbContext.ViewCountData.Where(vc => vc.OrgId == orgId).Select(vc => CountXML<ViewCountCol>.XMLtoDATA(vc.count));
            return counts;
        }
    }

   
}