using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Category methods created below
    }

    public interface ICategoryRepository : IRepository<Category>
    {
        //Methods for how when we need to get Categorys
    }
}