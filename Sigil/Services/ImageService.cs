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
        Image AssignDefaultImage(string userId);
        Image AssignDefaultImage(int id, ImageType imgtype);
        string GetIcon(string userId);
        string GetIcon(int orgId, ImageType imgtype);
        string GetBanner(int orgId, ImageType imgtype);
        Image GetUserImages(string userId);
        Image GetOrgImages(int orgId);
        Image GetTopicImages(int topicId);
        Image GetProductImages(int productId);
    }

    public class ImageService : IImageService
    {
        private static string default_folder_path = "/Images/Default/";
        private static string org_folder_path = "/Images/Org/";
        private static string user_folder_path = "/Images/User/";


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

        public Image AssignDefaultImage(string userId)
        {
          
            Image userImg = new Sigil.Models.Image();

            userImg.OwnerId = userId;
            userImg.imgType = (int)ImageType.User;

            CreateImage(userImg);
            SaveImage();

            userImg = imageRepository.GetUserImage(userId);
            return userImg;
            
        }

        public Image AssignDefaultImage(int id, ImageType imgType)
        {
            Image Img = new Sigil.Models.Image();

            
            Img.OwnerId = id.ToString();
            Img.imgType = (int)imgType;

            CreateImage(Img);
            SaveImage();
            if (imgType == ImageType.Org)
                Img = imageRepository.GetOrgImage(id);
            else if (imgType == ImageType.Product)
                Img = imageRepository.GetProductImage(id);
            else if (imgType == ImageType.Topic)
                Img = imageRepository.GetTopicImage(id);
            return Img;
        }

        public string GetIcon(string userId)
        {
            var userImg = imageRepository.GetUserImage(userId);
            if (userImg == default(Image))
            {
                return default_folder_path + "default1.png";
            }
            return userImg.icon_100;

        }

        public string GetIcon(int id, ImageType imgType)
        {
            Image Img = default(Image);
            if (imgType == ImageType.Org)
                Img = imageRepository.GetOrgImage(id);
            else if (imgType == ImageType.Product)
                Img = imageRepository.GetProductImage(id);
            else if (imgType == ImageType.Topic)
                Img = imageRepository.GetTopicImage(id);
            if (Img == default(Image))
            {
                return default_folder_path + "default1.png";
            }
            return Img.icon_100;
        }

        public string GetBanner(int id, ImageType imgType)
        {
            Image Img = default(Image);
            if (imgType == ImageType.Org)
                Img = imageRepository.GetOrgImage(id);
            else if (imgType == ImageType.Product)
                Img = imageRepository.GetProductImage(id);
            else if (imgType == ImageType.Topic)
                Img = imageRepository.GetTopicImage(id);
            if (Img == default(Image))
            {
                return default_folder_path + "default1.png";
            }
            return Img.banner;
        }

        public Image GetUserImages(string userId)
        {
            return imageRepository.GetUserImage(userId);
        }

        public Image GetOrgImages(int orgId)
        {
            return imageRepository.GetOrgImage(orgId);
        }

        public Image GetTopicImages(int topicId)
        {
            return imageRepository.GetTopicImage(topicId);
        }

        public Image GetProductImages(int productId)
        {
            return imageRepository.GetProductImage(productId);
        }

       
    }
}