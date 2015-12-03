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
        Subscription GetUserSubscriptionToProduct(string userId, int productId);
        Subscription GetUserSubscriptionToTopic(string userId, int topicId);
    }

    public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Subscription GetUserSubscriptionToOrg(string userId, int orgId)
        {
            var sub = this.DbContext.Subscriptions.Where(s => s.UserId == userId && s.OrgId == orgId).FirstOrDefault();
            return sub;
        }

        public Subscription GetUserSubscriptionToProduct(string userId, int productId)
        {
            return DbContext.Subscriptions.Where(s => s.UserId == userId && s.ProductId == productId).FirstOrDefault();
        }

        public Subscription GetUserSubscriptionToTopic(string userId, int topicId)
        {
            return DbContext.Subscriptions.Where(s => s.UserId == userId && s.TopicId == topicId).FirstOrDefault();
        }

        //where we define the Subscription methods created below
    }

    
}