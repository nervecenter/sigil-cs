using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        Subscription GetUserSubscriptionToOrg(string userId, int orgId);
    }

    public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Subscription GetUserSubscriptionToOrg(string userId, int orgId)
        {
            var sub = this.DbContext.Subscriptions.Where(s => s.UserId == userId && s.OrgId == orgId).FirstOrDefault();
            return sub;
        }

        //where we define the Subscription methods created below
    }

    
}