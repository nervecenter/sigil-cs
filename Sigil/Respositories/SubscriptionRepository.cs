using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Subscription methods created below
    }

    public interface ISubscriptionRepository : IRepository<Subscription>
    {
        //Methods for how when we need to get Subscriptions
    }
}