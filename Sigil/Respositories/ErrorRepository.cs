
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class ErrorRepository : RepositoryBase<Error>, IErrorRepository
    {
        public ErrorRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Error methods created below

    }

    public interface IErrorRepository : IRepository<Error>
    {
        //Methods for how when we need to get Errors


    }
}