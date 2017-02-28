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
        void UpdateIssue(Issue issue);
        void SaveChanges();
        void DeleteIssue(Issue issue);
        void ChangeIssueProduct(Issue issue, int newProductId);
        Issue GetIssue(int issueId);

        Issue GetLatestIssue(string userId, int productId);

        IEnumerable<Issue> GetAllIssues();
        IEnumerable<Issue> GetAllOrgIssues(int orgId);
        IEnumerable<Issue> GetAllTopicIssues(int topicId);
        IEnumerable<Issue> GetAllProductIssues(int productId);
        IEnumerable<Issue> GetAllUserIssues(string userId);
        IEnumerable<Issue> GetAllUserIssues(string userId, IEnumerable<Subscription> subs);
    }

    public class IssueService : IIssueService
    {
        private readonly IOrgRepository orgRepository;
        private readonly IIssueRepository issueRepository;
        private readonly IUnitOfWork unitOfWork;


        public IssueService(IIssueRepository issueRepo, IOrgRepository orgRepo, IUnitOfWork unitofwork)
        {
            issueRepository = issueRepo;
            orgRepository = orgRepo;

            unitOfWork = unitofwork;
        }


        public void CreateIssue(Issue issue)
        {
            issueRepository.Add(issue);    
        }

        public void UpdateIssue(Issue issue)
        {
            issueRepository.Update(issue);
        }

        public void SaveChanges()
        {
            unitOfWork.Commit();
        }

        public Issue GetIssue(int issueId)
        {
            return issueRepository.GetById(issueId) ?? default(Issue);
        }

        public Issue GetLatestIssue(string userId, int productId)
        {
            var userIssues = GetAllUserIssues(userId);

            return userIssues.Where(i => i.ProductId == productId).OrderByDescending(i => i.createTime).FirstOrDefault();
        }

        public IEnumerable<Issue> GetAllIssues()
        {
            return issueRepository.GetAll() ?? new List<Issue>().AsEnumerable();
        }

        public IEnumerable<Issue> GetAllOrgIssues(int orgId)
        {
            return issueRepository.GetMany(i => i.Product.OrgId == orgId) ?? new List<Issue>().AsEnumerable();
        }

        public IEnumerable<Issue> GetAllTopicIssues(int topicId)
        {
            return issueRepository.GetMany(i => i.Product.TopicId == topicId);
        }

        public IEnumerable<Issue> GetAllProductIssues(int proId)
        {
            return issueRepository.GetMany(i => i.ProductId == proId) ?? new List<Issue>().AsEnumerable();
        }

        public IEnumerable<Issue> GetAllUserIssues(string userId)
        {

            return issueRepository.GetMany(i => i.UserId == userId) ?? new List<Issue>().AsEnumerable();
        }

        public IEnumerable<Issue> GetAllUserIssues(string userid, IEnumerable<Subscription> subs)
        {
            var userSubmitted = issueRepository.GetMany(i => i.UserId == userid);
            List<Issue> userSubscribed = new List<Issue>();
            foreach(var s in subs)
            {
                if (s.OrgId.HasValue)
                    userSubscribed = userSubscribed.Union(GetAllOrgIssues(s.OrgId.Value)).ToList();
                else if (s.ProductId.HasValue)
                    userSubscribed = userSubscribed.Union(GetAllProductIssues(s.ProductId.Value)).ToList();
                else if (s.TopicId.HasValue)
                    userSubscribed = userSubscribed.Union(GetAllTopicIssues(s.TopicId.Value)).ToList();
            }
            return userSubscribed.Union(userSubmitted);
        }

        public void DeleteIssue(Issue issue)
        {
            issueRepository.Delete(issue);
            unitOfWork.Commit();
        }

        public void ChangeIssueProduct(Issue issue, int newProductId)
        {
            issue.ProductId = newProductId;
            UpdateIssue(issue);
            SaveChanges();
        }
    }
}