using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    public interface IOrgRepository : IRepository<Org>
    {
        Org GetByName(string orgName);
        Org GetByURL(string orgURL);
    }

    public class OrgRepository : RepositoryBase<Org>, IOrgRepository
    {
        public OrgRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Org GetByName(string orgName)
        {
            var org = this.DbContext.Orgs.Where(o => o.orgName == orgName).FirstOrDefault();
            return org;
        }

        public Org GetByURL(string orgURL)
        {
            var org = this.DbContext.Orgs.Where(o => o.orgURL == orgURL).FirstOrDefault();
            return org;
        }
    }

    public interface IOrgAppRepository : IRepository<OrgApp>
    {

    }

    public class OrgApplicantRepository : RepositoryBase<OrgApp>, IOrgAppRepository
    {
        public OrgApplicantRepository(IDbFactory dbFactory) : base(dbFactory) { }
    }


}