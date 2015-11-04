using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Category methods created below
        public Category GetCategoryById(int orgId, int catId)
        {
            var cat = this.DbContext.Categories.Where(c => c.orgId == orgId && c.Id == catId).FirstOrDefault();
            return cat;
        }
    }

    public interface ICategoryRepository : IRepository<Category>
    {
        //Methods for how when we need to get Categorys
        Category GetCategoryById(int orgid, int catid);
    }
}