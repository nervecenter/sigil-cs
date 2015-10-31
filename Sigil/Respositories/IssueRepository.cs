using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class IssueRepository : RepositoryBase<Issue>, IIssueRepository
    {
        public IssueRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the issue methods created below
    }

    public interface IIssueRepository : IRepository<Issue>
    {
        //Methods for how when we need to get issues
    }
}