using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Comment methods created below
    }

    public interface ICommentRepository : IRepository<Comment>
    {
        //Methods for how when we need to get Comments
    }
}