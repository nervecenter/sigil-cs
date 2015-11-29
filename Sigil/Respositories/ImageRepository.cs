using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Repository
{

    public interface IImageRepository : IRepository<Image>
    {
        //Methods for how when we need to get Images

        Image GetUserImage(string userId);
        Image GetOrgImage(int orgId);
        Image GetProductImage(int productId);
        Image GetTopicImage(int topId);
    }

    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Image GetOrgImage(int orgId)
        {
            return DbContext.Images.Where(i => i.OwnerId == orgId.ToString() && i.imgType == (int)ImageType.Org).FirstOrDefault();
        }

        public Image GetProductImage(int productId)
        {
            return DbContext.Images.Where(i => i.OwnerId == productId.ToString() && i.imgType == (int)ImageType.Product).FirstOrDefault();
        }

        public Image GetTopicImage(int topId)
        {
            return DbContext.Images.Where(i => i.OwnerId == topId.ToString() && i.imgType == (int)ImageType.Topic).FirstOrDefault();
        }

        public Image GetUserImage(string userId)
        {
            return DbContext.Images.Where(i => i.OwnerId == userId && i.imgType == (int)ImageType.User).FirstOrDefault();
        }

        //where we define the Image methods created below
    }

   
}