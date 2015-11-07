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

        //where we define the Subscription methods created below
    }

    
}