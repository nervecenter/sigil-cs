using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


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

    }

    public class OrgService : IOrgService
    {
        private readonly IOrgRepository OrgsRepository;
        private readonly IOrgAppRepository OrgAppRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IIssueRepository issueRepository;
        private readonly ICountRepository countRespository;
        private readonly ICommentRepository commentRespository;
        private readonly IUserRepository userRespository;
        private readonly IUnitOfWork unitOfWork;

        public OrgService(IOrgRepository orgRepo, IUnitOfWork unitofwork)
        {
            this.OrgsRepository = orgRepo;
            this.unitOfWork = unitofwork;
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
            var orgs = OrgsRepository.GetMany(o => o.topicid == topicId);
            return orgs;
        }

        public void CreateOrg(Org org)
        {
            OrgsRepository.Add(org);
        }

        public void SaveOrg()
        {
            unitOfWork.Commit();
        }


    }
}