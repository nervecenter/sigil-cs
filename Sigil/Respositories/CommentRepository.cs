using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{

    public interface ICommentRepository : IRepository<Comment>
    {
        //Methods for how when we need to get Comments

        Comment GetById(int orgId, int issueId, int commentId);
        IEnumerable<Comment> GetIssueComments(int orgId, int issueId);

    }

    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Comment methods created below

        public Comment GetById(int orgId, int issueId, int commentId)
        {
            var com = this.DbContext.Comments.Where(c => c.Issue.Category.OrgId == orgId && c.IssueId == issueId && c.Id == commentId).FirstOrDefault();
            return com;
        }

        public IEnumerable<Comment> GetIssueComments(int orgId, int issueId)
        {
            var com = this.DbContext.Comments.Where(c => c.Issue.Category.OrgId == orgId && c.IssueId == issueId).Select(c => c);
            return com;
        }
    }
}