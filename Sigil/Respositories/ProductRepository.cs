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
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Product GetProductByName(string product)
        {
            return DbContext.Products.Where(p => p.ProductName == product).FirstOrDefault();
        }

        public Product GetProductByURL(string product)
        {
            return DbContext.Products.Where(p => p.ProductURL == product).FirstOrDefault();
        }

        //where we define the Product methods created below

    }

   
}