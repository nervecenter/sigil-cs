using Sigil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IOfficialResponseService
    {

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