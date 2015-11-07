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

        Subscription GetUserSubscription(string userId, int orgId);

        //Tuple<IEnumerable<Org>, IEnumerable<Topic>, IEnumerable<Category>> GetUserSubscriptions(string userId);
        IEnumerable<Subscription> GetUserSubscriptions(string userId);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly ICategoryRepository categoryRepository;
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

        public Subscription GetUserSubscription(string userId, int orgId)
        {
            return subscriptionRepository.GetUserSubscriptionToOrg(userId, orgId);
        }

        public IEnumerable<Subscription> GetUserSubscriptions(string userId)
        {
            return subscriptionRepository.GetMany(s => s.UserId == userId);
        }
    }
}