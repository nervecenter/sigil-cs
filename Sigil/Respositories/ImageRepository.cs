using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Sigil.Models;

namespace Sigil.Models
{
    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(IDbFactory dbFactory) : base(dbFactory) { }

        //where we define the Image methods created below
    }

    public interface IImageRepository : IRepository<Image>
    {
        //Methods for how when we need to get Images
    }
}