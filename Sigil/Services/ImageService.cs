using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;

namespace Sigil.Services
{
    //The operations we want to expose to the controllers
    public interface IImageService
    {
        //void CreateImage(Image img);
        void SaveImage();
        void AssignDefaultImage(string userId);
        string GetIcon(string userId);
        string GetIcon(int orgId);
        string GetBanner(int orgId);

    }

    public class ImageService : IImageService
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