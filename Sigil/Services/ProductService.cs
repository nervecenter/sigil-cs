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
        void CreateProduct(Product prod);
        void UpdateProduct(Product prod);
        void SaveProduct();
        void DeleteProduct(Product prod);
        IEnumerable<Product> GetAllProducts();

        IEnumerable<Product> GetProductsByOrg(int orgId);
        IEnumerable<Product> GetProductsByOrg(string org, bool name = false);
        Product GetProduct(int productId);
        Product GetProduct(string productStr, bool name = false);
        Product GetProduct(string productStr, int orgId, bool name = false);
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

        public void CreateProduct(Product prod)
        {
            productRepository.Add(prod);
        }

        public void SaveProduct()
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

        public Product GetProduct(int productId)
        {
            var cat = productRepository.GetById(productId);
            return cat ?? default(Product);
        }

        public Product GetProduct(string productStr, int orgId, bool name)
        {
            Product product = default(Product);
            if (name)
                product = productRepository.GetProductByName(productStr, orgId);
            else
                product = productRepository.GetProductByURL(productStr, orgId);

            return product;
        }

        /// <summary>
        /// Gets product by either the products Name or the product URL
        /// </summary>
        /// <param name="productStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Product GetProduct(string productStr, bool name)
        {
            Product product = default(Product);
            if (name)
                product = productRepository.GetProductByName(productStr);
            else
                product = productRepository.GetProductByURL(productStr);

            return product;
        }

        public void UpdateProduct(Product prod)
        {
            productRepository.Update(prod);
            unitOfWork.Commit();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return productRepository.GetAll();
        }

        public void DeleteProduct(Product prod)
        {
            productRepository.Delete(prod);
            unitOfWork.Commit();
        }


    }
}