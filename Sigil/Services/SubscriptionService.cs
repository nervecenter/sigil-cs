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

        Tuple<IEnumerable<Org>, IEnumerable<Topic>, IEnumerable<Category>> GetUserSubscriptions(string userId);

    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICountRepository countRespository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;




    }
}