using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface ISubscriptionService
    {
        void CreateSubscription(Subscription sub);
        void SaveSubscription();

        void RemoveSubscription(Subscription sub);

        Subscription GetUserOrgSubscription(string userId, int orgId);
        Subscription GetUserProductSubscription(string userId, int productId);
        Subscription GetUserTopicSubscription(string userId, int topicId);

        //Tuple<IEnumerable<Org>, IEnumerable<Topic>, IEnumerable<Product>> GetUserSubscriptions(string userId);
        IEnumerable<Subscription> GetUserSubscriptions(string userId);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly IProductRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;

        public SubscriptionService(IUnitOfWork unit, ISubscriptionRepository subRepo)
        {
            unitOfWork = unit;
            subscriptionRepository = subRepo;
        }

        public void CreateSubscription(Subscription sub)
        {
            subscriptionRepository.Add(sub);
        }

        public void SaveSubscription()
        {
            unitOfWork.Commit();
        }

        public void RemoveSubscription(Subscription sub)
        {
            subscriptionRepository.Delete(sub);
        }

        public Subscription GetUserProductSubscription(string userId, int productId)
        {
            return subscriptionRepository.GetUserSubscriptionToProduct(userId, productId);
        }

        public Subscription GetUserTopicSubscription(string userId, int topicId)
        {
            return subscriptionRepository.GetUserSubscriptionToTopic(userId, topicId);
        }

        public Subscription GetUserOrgSubscription(string userId, int orgId)
        {
            return subscriptionRepository.GetUserSubscriptionToOrg(userId, orgId) ?? default(Subscription);
        }

        public IEnumerable<Subscription> GetUserSubscriptions(string userId)
        {
            return subscriptionRepository.GetMany(s => s.UserId == userId) ?? new List<Subscription>().AsEnumerable();
        }
    }
}