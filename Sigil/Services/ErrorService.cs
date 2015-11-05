using Sigil.Models;
using Sigil.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IErrorService
    {
        IEnumerable<Error> GetAllErrors();
        void CreateError(object errorObj, string msg);
        void CreateError(object errorObj, Exception e, string msg = "");

        //Creating an error automatically saves it as well
        //void SaveError();

    }

    public class ErrorService : IErrorService
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