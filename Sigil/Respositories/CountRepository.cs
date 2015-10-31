using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class CountRepository : RepositoryBase<Count>, ICountRepository
    {
        public CountRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Count methods created below
    }

    public interface ICountRepository : IRepository<Count>
    {
        //Methods for how when we need to get Counts
    }
}