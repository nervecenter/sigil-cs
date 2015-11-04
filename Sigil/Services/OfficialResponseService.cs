using Sigil.Models;
using Sigil.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IOfficialResponseService
    {
        void CreateOfficialResponse(OfficialResponse offRes);
        void SaveOfficialResponse();
    }

    public class OfficialResponseService : IErrorService
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