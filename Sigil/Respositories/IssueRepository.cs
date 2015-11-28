using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    

    public interface IIssueRepository : IRepository<Issue>
    {
        //Methods for how when we need to get issues

        Issue GetById(int orgId, int productId, int issueId);
    }

    public class IssueRepository : RepositoryBase<Issue>, IIssueRepository
    {
        public IssueRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Issue GetById(int orgId, int productId, int issueId)
        {
            return this.DbContext.Issues.Where(i => i.Product.OrgId == orgId && i.Product.Id == productId && i.Id == issueId).FirstOrDefault();
        }

        //where we define the issue methods created below
    }
}