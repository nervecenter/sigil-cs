using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;
using System.Diagnostics;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IProductService
    {
        void CreateCategory(Product cat);
        void SaveCategory();
        IEnumerable<Product> GetProductsByOrg(int orgId);
        IEnumerable<Product> GetProductsByOrg(string org, bool name = false);
        Product GetProduct(int orgId, int catId);
        Product GetProduct(int orgId, string cat);
    }

    public class ProductService : IProductService
    {

        private readonly IProductRepository productRepository;
        private readonly IOrgRepository orgRepository;
        private readonly IUnitOfWork unitOfWork;

        private readonly IErrorService errorService;

        public ProductService(IProductRepository proRepo,IOrgRepository orgRepo, IErrorService errS,IUnitOfWork unit)
        {
            productRepository = proRepo;
            orgRepository = orgRepo;
            errorService = errS;
            unitOfWork = unit;
        }

        public void CreateCategory(Product cat)
        {
            productRepository.Add(cat);
        }

        public void SaveCategory()
        {
            unitOfWork.Commit();
        }

        public IEnumerable<Product> GetProductsByOrg(int orgId)
        {
            var pros = productRepository.GetMany(c => c.OrgId == orgId);
            if(pros == null)
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(orgId, "Org does not have default product assigned./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Critical);
            }
            return pros ?? new List<Product>().AsEnumerable();
        }

        public IEnumerable<Product> GetProductsByOrg(string orgS, bool name)
        {
            
            IEnumerable<Product> pros;
            if (name)
            {
                var org = orgRepository.GetByName(orgS);
                pros = productRepository.GetMany(c => c.OrgId == org.Id);
            }
            else
            {
                var org = orgRepository.GetByURL(orgS);
                pros = productRepository.GetMany(c => c.OrgId == org.Id);
            }

            if (pros == null)
            {
                StackTrace stack = new StackTrace();
                errorService.CreateError(name, "Org does not have default product assigned./n" + stack.GetFrame(1).GetMethod().Name, ErrorLevel.Critical);
            }

            return pros ?? new List<Product>().AsEnumerable();
        }

        public Product GetProduct(int orgId, int catId)
        {
            var cat = productRepository.GetProductById(orgId, catId);
            return cat ?? default(Product);
        }

        public Product GetProduct(int orgId, string cat)
        {
            return productRepository.GetProductByName(orgId, cat) ?? default(Product);
        }
    }
}