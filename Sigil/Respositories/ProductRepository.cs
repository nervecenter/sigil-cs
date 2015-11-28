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
        Product GetProductById(int orgid, int catid);
        Product GetProductByName(int orgId, string catName);
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Product methods created below
        public Product GetProductById(int orgId, int catId)
        {
            var cat = this.DbContext.Categories.Where(c => c.OrgId == orgId && c.Id == catId).FirstOrDefault();
            return cat;
        }

        public Product GetProductByName(int orgId, string catName)
        {
            var cat = this.DbContext.Categories.Where(c => c.OrgId == orgId && c.ProductName == catName).FirstOrDefault();
            return cat;
        }

    }

   
}