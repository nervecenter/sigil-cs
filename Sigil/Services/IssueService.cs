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

        Issue GetIssue(int orgId, int issueId);
        Issue GetIssue(string org, int issueId);

        Issue GetLatestIssue(string userId, int orgId);

        IEnumerable<Issue> GetAllIssues();
        IEnumerable<Issue> GetAllOrgIssues(int orgId);
        IEnumerable<Issue> GetAllTopicIssues(int topicId);
        IEnumerable<Issue> GetAllCategoryIssues(int orgId, int catId);
        IEnumerable<Issue> GetAllUserIssues(string userId);

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

        public Issue GetIssue(int orgId, int issueId)
        {
            return issueRepository.GetById(orgId, issueId);
        }

        public Issue GetIssue(string org, int issueId)
        {
            var orgI = orgRepository.GetByName(org);
            return issueRepository.GetById(orgI.Id, issueId);
        }

        public Issue GetLatestIssue(string userId, int orgId)
        {
            var userIssues = GetAllUserIssues(userId);

            return userIssues.Where(i => i.Category.OrgId == orgId).OrderByDescending(i => i.createTime).FirstOrDefault();
        }

        public IEnumerable<Issue> GetAllIssues()
        {
            return issueRepository.GetAll();
        }

        public IEnumerable<Issue> GetAllOrgIssues(int orgId)
        {
            return issueRepository.GetMany(i => i.Category.OrgId == orgId);
        }

        public IEnumerable<Issue> GetAllTopicIssues(int topicId)
        {
            throw new NotImplementedException();
            //return issueRepository.GetMany(i => i.TopicId == topicId);
        }

        public IEnumerable<Issue> GetAllCategoryIssues(int orgId, int catId)
        {
            return issueRepository.GetMany(i => i.Category.OrgId == orgId && i.CatId == catId);
        }

        public IEnumerable<Issue> GetAllUserIssues(string userId)
        {
            return issueRepository.GetMany(i => i.UserId == userId);
        }
    }
}