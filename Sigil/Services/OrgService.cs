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
        void CreateOrg(Org org);
        void SaveOrg();

        Org GetOrg(int id);


        /// <summary>
        /// Get the category by orgURL or by orgName. Checks by looking for url first, if no results then checks by name. 
        /// </summary>
        /// <param name="org">Either the orgURL or name</param>
        /// <param name="name">if True then only looks by name</param>
        /// <returns></returns>
        Org GetOrg(string org, bool name);
        IEnumerable<Org> GetTopicOrgs(int topicId);

    }

    public class OrgService : IOrgService
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