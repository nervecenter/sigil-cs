using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IIssueService
    {
        void CreateIssue(Issue issue);
        void SaveIssue();

        Issue GetIssue(int orgId, int issueId);
        IEnumerable<Issue> GetAllIssues();
        IEnumerable<Issue> GetOrgIssues(int orgId);
        IEnumerable<Issue> GetTopicIssues(int topicId);
        IEnumerable<Issue> GetCategoryIssues(int catId);
        IEnumerable<Issue> GetUserIssues(string userId);

    }

    public class IssueService : IIssueService
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