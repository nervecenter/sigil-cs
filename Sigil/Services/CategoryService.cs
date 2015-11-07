using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface ICategoryService
    {
        void CreateCategory(Category cat);
        void SaveCategory();
        IEnumerable<Category> GetCategoriesByOrg(int orgId);
        IEnumerable<Category> GetCategoriesByOrg(string org, bool name = false);
        Category GetCategory(int orgId, int catId);
        Category GetCategory(int orgId, string cat);
    }

    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        public CategoryService(ICategoryRepository catRepo, IUnitOfWork unit)
        {
            this.categoryRepository = catRepo;
            this.unitOfWork = unit;
        }

        public void CreateCategory(Category cat)
        {
            categoryRepository.Add(cat);
        }

        public void SaveCategory()
        {
            unitOfWork.Commit();
        }

        public IEnumerable<Category> GetCategoriesByOrg(int orgId)
        {
            var cats = categoryRepository.GetMany(c => c.orgId == orgId);
            return cats;
        }

        public IEnumerable<Category> GetCategoriesByOrg(string org, bool name)
        {
            IEnumerable<Category> cats;
            if(name)
                cats = categoryRepository.GetMany(c => c.Org.orgName == org);
            else
                cats = categoryRepository.GetMany(c => c.Org.orgURL == org || c.Org.orgName == org);

            return cats;
        }

        public Category GetCategory(int orgId, int catId)
        {
            var cat = categoryRepository.GetCategoryById(orgId, catId);
            return cat;
        }

        public Category GetCategory(int orgId, string cat)
        {
            return categoryRepository.GetCategoryByName(orgId, cat);
        }
    }
}