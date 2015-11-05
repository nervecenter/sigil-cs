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
        
        IEnumerable<AspNetUser> MatchUsersByName(string term);
        IEnumerable<Issue> MatchIssuesByTitle(string term);
    }

    public class SearchService: ISearchService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly IOrgAppRepository OrgAppRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICountRepository countRespository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;


    }
}
