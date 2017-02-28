using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{

    public interface IProductRepository : IRepository<Product>
    {
        //Methods for how when we need to get Products
        Product GetProductByName(string product);
        Product GetProductByURL(string product);
        Product GetProductByName(string product, int orgId);
        Product GetProductByURL(string product, int orgId);
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Product GetProductByName(string product)
        {
            return DbContext.Products.Where(p => p.ProductName == product).FirstOrDefault();
        }

        public Product GetProductByName(string product, int orgId)
        {
            return DbContext.Products.Where(p => p.ProductName == product && p.OrgId == orgId).FirstOrDefault();
        }

        public Product GetProductByURL(string product)
        {
            return DbContext.Products.Where(p => p.ProductURL == product).FirstOrDefault();
        }

        public Product GetProductByURL(string product, int orgId)
        {
            return DbContext.Products.Where(p => p.ProductURL == product && p.OrgId == orgId).FirstOrDefault();
        }

        //where we define the Product methods created below

    }

   
}