﻿using System;
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

        IEnumerable<Issue> MatchIssuesByTitle(string term);
    }

    public class SearchService: ISearchService
    {
        private readonly IOrgRepository orgRepository;
        private readonly IProductRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ITopicRepository topicRepository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        public SearchService(IUnitOfWork unit, IOrgRepository orgRepo, IUserRepository userRepo, IIssueRepository issRepo, ITopicRepository topRepo, IProductRepository catRepo)
        {
            unitOfWork = unit;
            orgRepository = orgRepo;
            userRepository = userRepo;
            issueRepository = issRepo;
            topicRepository = topRepo;
            categoryRepository = catRepo;
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
    }
}