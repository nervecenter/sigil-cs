using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;


namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IOrgService
    {
        void CreateOrgApp(OrgApp newOrg);
        void SaveOrgApp();

        OrgApp ApproveOrgApp(int orgAppId);

        void CreateOrg(Org org);
        void SaveOrg();

        Org GetOrg(int id);

        IEnumerable<OrgApp> GetAllOrgApplicants();

        /// <summary>
        /// Get the category by orgURL or by orgName. Checks by looking for url first, if no results then checks by name. 
        /// </summary>
        /// <param name="org">Either the orgURL or name</param>
        /// <param name="name">if True then only looks by name</param>
        /// <returns></returns>
        Org GetOrg(string org, bool name = false);
        IEnumerable<Org> GetTopicOrgs(int topicId);

        IEnumerable<Org> GetAllOrgs();
    }

    public class OrgService : IOrgService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly IOrgAppRepository orgAppRepository;
        private readonly IProductRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;

        public OrgService(IOrgRepository orgRepo, IOrgAppRepository orgAppRepo,IUnitOfWork unitofwork)
        {
            OrgsRepository = orgRepo;
            orgAppRepository = orgAppRepo;
            unitOfWork = unitofwork;
        }

        public Org GetOrg(string orgstr, bool name)
        {
            Org org = default(Org);
            if (!name)
                org = OrgsRepository.GetByName(orgstr);

            if (org != default(Org))
                return org;

            org = OrgsRepository.GetByURL(orgstr);
            return org;
        }

        public Org GetOrg(int id)
        {
            var org = OrgsRepository.GetById(id);

            return org;
        }

        public IEnumerable<Org> GetTopicOrgs(int topicId)
        {
            //var orgs = OrgsRepository.GetMany(o => o == topicId);
            //return orgs;
            throw new NotImplementedException();
        }

        public void CreateOrg(Org org)
        {
            OrgsRepository.Add(org);
        }

        public void SaveOrg()
        {
            unitOfWork.Commit();
        }

        public void CreateOrgApp(OrgApp newOrg)
        {
            orgAppRepository.Add(newOrg);
        }

        public void SaveOrgApp()
        {
            unitOfWork.Commit();
        }

        public OrgApp ApproveOrgApp(int orgAppId)
        {
            return orgAppRepository.GetById(orgAppId);
        }

        public IEnumerable<OrgApp> GetAllOrgApplicants()
        {
            return orgAppRepository.GetAll();
        }

        public IEnumerable<Org> GetAllOrgs()
        {
            return OrgsRepository.GetAll();
        }
    }
}