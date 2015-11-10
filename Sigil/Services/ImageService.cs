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
        void CreateImage(Image img);
        void SaveImage();
        void AssignDefaultImage(string userId);
        string GetIcon(string userId);
        string GetIcon(int orgId);
        string GetBanner(int orgId);
        Image GetUserImages(string userId);
        Image GetOrgImages(int orgId);
        Image GetTopicImages(int topicId);
        Image GetCategoryImages(int orgId, int catId);
    }

    public class ImageService : IImageService
    {
        
        private readonly IImageRepository imageRepository;
        private readonly IUnitOfWork unitOfWork;

        public ImageService(IUnitOfWork unit, IImageRepository imgRepo)
        {
            unitOfWork = unit;
            imageRepository = imgRepo;
        }

        public void CreateImage(Image img)
        {
            imageRepository.Add(img);
        }

        public void SaveImage()
        {
            unitOfWork.Commit();
        }

        public void AssignDefaultImage(string userId)
        {
            int defaultIMG = RNG.RandomNumber(1, 6);

            string IMG_PATH = "default" + defaultIMG + ".png";
            Image userImg = new Sigil.Models.Image();


            //userImg.UserId = userId;
            userImg.icon_100 = IMG_PATH;

            CreateImage(userImg);
            SaveImage();
            
        }

        public string GetIcon(string userId)
        {
            var userImg = imageRepository.GetImageByUserId(userId);
            return userImg.icon_100;

        }

        public string GetIcon(int orgId)
        {
            var orgImg = imageRepository.GetImageByOrgId(orgId);
            return orgImg.icon_100;
        }

        public string GetBanner(int orgId)
        {
            var orgImg = imageRepository.GetImageByOrgId(orgId);
            return orgImg.banner;
        }

        public Image GetUserImages(string userId)
        {
            return imageRepository.GetImageByUserId(userId);
        }

        public Image GetOrgImages(int orgId)
        {
            return imageRepository.GetImageByOrgId(orgId);
        }

        public Image GetTopicImages(int topicId)
        {
            return imageRepository.GetImageByTopicId(topicId);
        }

        public Image GetCategoryImages(int orgId, int catId)
        {
            return imageRepository.GetImageByOrgId(orgId, catId);
        }
    }
}