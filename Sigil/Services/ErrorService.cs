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
        private readonly IErrorRepository errorRepository;
        private readonly IUnitOfWork unitOfWork;


        public ErrorService(IUnitOfWork unit, IErrorRepository errorRepo)
        {
            unitOfWork = unit;
            errorRepository = errorRepo;
        }

        public IEnumerable<Error> GetAllErrors()
        {
            return errorRepository.GetAll();
        }

        public void CreateError(object errorObj, string msg)
        {
            Error newErr = new Error();
            newErr.error_date = DateTime.UtcNow;
            newErr.error_object = errorObj.ToString();
            newErr.error_exception = msg;

            errorRepository.Add(newErr);

            unitOfWork.Commit();
        }

        public void CreateError(object errorObj, Exception e, string msg = "")
        {
            Error newErr = new Error();
            newErr.error_date = DateTime.UtcNow;
            newErr.error_object = errorObj.ToString();
            newErr.error_exception = e.Message + '\n'+ msg;

            errorRepository.Add(newErr);

            unitOfWork.Commit();
        }
    }
}