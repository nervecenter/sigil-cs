using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Repository;
using Sigil.Models;

namespace Sigil.Services
{
    public interface ISearchService
    {
        IEnumerable<Org> MatchOrgsByName(string term);
        
        IEnumerable<ApplicationUser> MatchUsersByName(string term);

        IEnumerable<Topic> MatchTopicsByName(string term);

        IEnumerable<Product> MatchProductsByName(string term);

        IEnumerable<Issue> MatchIssuesByTitle(string term);

        IEnumerable<Issue> MatchIssuesByOrg( int id );

        IEnumerable<Issue> MatchIssuesByProduct( int id );

        IEnumerable<Issue> MatchIssuesByTopic( int id );
    }

    public class SearchService: ISearchService
    {
        private readonly IOrgRepository orgRepository;
        private readonly IProductRepository productRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ITopicRepository topicRepository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        public SearchService(IUnitOfWork unit, IOrgRepository orgRepo, IUserRepository userRepo, IIssueRepository issRepo, ITopicRepository topRepo, IProductRepository prodRepo)
        {
            unitOfWork = unit;
            orgRepository = orgRepo;
            userRepository = userRepo;
            issueRepository = issRepo;
            topicRepository = topRepo;
            productRepository = prodRepo;
        }

        public IEnumerable<Org> MatchOrgsByName(string term)
        {
            return orgRepository.GetMany(o => o.orgName.StartsWith(term));
        }

        public IEnumerable<ApplicationUser> MatchUsersByName(string term)
        {
            return userRepository.GetMany(u => u.DisplayName.StartsWith(term));
        }

        public IEnumerable<Issue> MatchIssuesByTitle(string term)
        {
            return issueRepository.GetMany(i => i.title.StartsWith(term));
        }

        public IEnumerable<Issue> MatchIssuesByOrg( int id ) 
        {
            return issueRepository.GetMany( i => i.Product.OrgId.Equals( id ) );
        }

        public IEnumerable<Issue> MatchIssuesByProduct( int id ) 
        {
            return issueRepository.GetMany( i => i.ProductId.Equals( id ) );
        }

        public IEnumerable<Issue> MatchIssuesByTopic( int id ) 
        {
            return issueRepository.GetMany( i => i.Product.TopicId.Equals( id ) );
        }

        public IEnumerable<Topic> MatchTopicsByName(string term)
        {
            return topicRepository.GetMany(t => t.topicName.StartsWith(term));
        }

        public IEnumerable<Product> MatchProductsByName(string term)
        {
            return productRepository.GetMany(p => p.ProductName.StartsWith(term));
        }
    }
}
