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
        private readonly IOrgRepository orgRepository;
        private readonly IUnitOfWork unitOfWork;

        public CategoryService(ICategoryRepository catRepo,IOrgRepository orgRepo, IUnitOfWork unit)
        {
            this.categoryRepository = catRepo;
            orgRepository = orgRepo;
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
            var cats = categoryRepository.GetMany(c => c.OrgId == orgId);
            return cats;
        }

        public IEnumerable<Category> GetCategoriesByOrg(string orgS, bool name)
        {
            
            IEnumerable<Category> cats;
            if (name)
            {
                var org = orgRepository.GetByName(orgS);
                cats = categoryRepository.GetMany(c => c.OrgId == org.Id);
            }
            else
            {
                var org = orgRepository.GetByURL(orgS);
                cats = categoryRepository.GetMany(c => c.OrgId == org.Id);
            }
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