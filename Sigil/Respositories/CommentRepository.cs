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

        
        IEnumerable<Comment> GetIssueComments(int issueId);

    }

    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Comment methods created below

        public IEnumerable<Comment> GetIssueComments(int issueId)
        {
            var com = this.DbContext.Comments.Where(c => c.IssueId == issueId).Select(c => c);
            return com;
        }
    }
}