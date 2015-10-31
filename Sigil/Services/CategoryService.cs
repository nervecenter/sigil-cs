using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;


namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface ICategoryService
    {
        void CreateCategory(Category cat);
        void SaveCategory();
        IEnumerable<Category> GetCategoriesByOrg(int orgId);
        IEnumerable<Category> GetCategoriesByOrg(string org, bool name);
        Category GetOrgCategory(int orgId, int catId);

    }

    public class CategoryService : ICategoryService
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