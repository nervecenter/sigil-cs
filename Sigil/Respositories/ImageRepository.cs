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

        Image GetImageByUserId(string userId);
        Image GetImageByOrgId(int orgId, int catId=0);

        Image GetImageByTopicId(int topId);
    }

    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(IDbFactory dbFactory) : base(dbFactory) { }

        public Image GetImageByOrgId(int orgId, int catId = 0)
        {
            throw new NotImplementedException();
            //var img = this.DbContext.Images.Where(i => i.OrgId == orgId && i.CatId == catId).FirstOrDefault();
            //return img;
        }

        public Image GetImageByTopicId(int topId)
        {
            throw new NotImplementedException();
            //var img = this.DbContext.Images.Where(i => i.TopicId == topId).FirstOrDefault();
            //return img;
        }

        public Image GetImageByUserId(string userId)
        {
            throw new NotImplementedException();
            //var img = this.DbContext.Images.Where(i => i.UserId == userId).FirstOrDefault();
            //return img;
        }

        //where we define the Image methods created below
    }

   
}