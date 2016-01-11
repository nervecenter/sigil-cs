using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sigil.Models;
using Sigil.Repository;
using ImageProcessor.Imaging.Formats;
using System.IO;
using ImageProcessor;
using System.ComponentModel;

namespace Sigil.Services
{
    enum ImageTypeSize
    {
        [Description("icon_100")]
        icon_100,
        [Description("icon_20")]
        icon_20,
        [Description("banner")]
        banner
    };

    //The operations we want to expose to the controllers
    public interface IImageService
    {

        void UserIconImageUpload(ApplicationUser user, HttpPostedFileBase img);

        void OrgBannerImageUpload(Org org,HttpPostedFileBase img);
        void OrgIcon20ImageUpload(Org org, HttpPostedFileBase img);
        void OrgIcon100ImageUpload(Org org, HttpPostedFileBase img);

        void ProductBannerImageUpload(Product product, HttpPostedFileBase img);
        void ProductIcon20ImageUpload(Product product, HttpPostedFileBase img);
        void ProductIcon100ImageUpload(Product product, HttpPostedFileBase img);

        void TopicBannerImageUpload(Topic topic, HttpPostedFileBase img);
        void TopicIcon20ImageUpload(Topic topic, HttpPostedFileBase img);
        void TopicIcon100ImageUpload(Topic topic, HttpPostedFileBase img);


        void CreateImage(Image img);
        void SaveImage();
        Image AssignDefaultImage(string userId);
        Image AssignDefaultImage(int id, ImageTypeOwner imgtype);
        string GetIcon(string userId);
        string GetIcon(int orgId, ImageTypeOwner imgtype);
        string GetBanner(int orgId, ImageTypeOwner imgtype);
        Image GetUserImages(string userId);
        Image GetOrgImages(int orgId);
        Image GetTopicImages(int topicId);
        Image GetProductImages(int productId);
    }

    public class ImageService : IImageService
    {
        private readonly static string tmp_upload_path = "~/Images/TMP/";
        private readonly static string default_folder_path = "/Images/Default/";
        private readonly static string org_folder_path = "/Images/Org/";
        private readonly static string user_folder_path = "/Images/User/";
        private readonly static string product_folder_path = "/Images/Product/";
        private readonly static string topic_folder_path = "/Images/Topic/"; 


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
            userImg.imgType = (int)ImageTypeOwner.User;

            CreateImage(userImg);
            SaveImage();

            userImg = imageRepository.GetUserImage(userId);
            return userImg;
            
        }

        public Image AssignDefaultImage(int id, ImageTypeOwner imgType)
        {
            Image Img = new Sigil.Models.Image();

            
            Img.OwnerId = id.ToString();
            Img.imgType = (int)imgType;

            CreateImage(Img);
            SaveImage();
            if (imgType == ImageTypeOwner.Org)
                Img = imageRepository.GetOrgImage(id);
            else if (imgType == ImageTypeOwner.Product)
                Img = imageRepository.GetProductImage(id);
            else if (imgType == ImageTypeOwner.Topic)
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

        public string GetIcon(int id, ImageTypeOwner imgType)
        {
            Image Img = default(Image);
            if (imgType == ImageTypeOwner.Org)
                Img = imageRepository.GetOrgImage(id);
            else if (imgType == ImageTypeOwner.Product)
                Img = imageRepository.GetProductImage(id);
            else if (imgType == ImageTypeOwner.Topic)
                Img = imageRepository.GetTopicImage(id);
            if (Img == default(Image))
            {
                return default_folder_path + "default1.png";
            }
            return Img.icon_100;
        }

        public string GetBanner(int id, ImageTypeOwner imgType)
        {
            Image Img = default(Image);
            if (imgType == ImageTypeOwner.Org)
                Img = imageRepository.GetOrgImage(id);
            else if (imgType == ImageTypeOwner.Product)
                Img = imageRepository.GetProductImage(id);
            else if (imgType == ImageTypeOwner.Topic)
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



        private string TMPImageHandler(HttpPostedFileBase img, string owner)
        {
            string img_path = HttpContext.Current.Server.MapPath(tmp_upload_path) + owner +"_"+ DateTime.Now.Second.ToString() + "_" + img.FileName;
            
            img.SaveAs(img_path);

            return img_path;
        }

        private string ImageConvert(string file, string owner, ImageTypeSize typeSize, ImageTypeOwner typeOwner)
        {
            byte[] img_bytes = System.IO.File.ReadAllBytes(file);
            ISupportedImageFormat format = new PngFormat();
            System.Drawing.Size size = new System.Drawing.Size();

            if (typeSize == ImageTypeSize.icon_100)
                size = new System.Drawing.Size(100, 100);
            else if (typeSize == ImageTypeSize.icon_20)
                size = new System.Drawing.Size(20, 20);
            else if (typeSize == ImageTypeSize.banner)
                size = new System.Drawing.Size(1000, 200);

            using (MemoryStream inStream = new MemoryStream(img_bytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (ImageFactory imgFact = new ImageFactory(preserveExifData: true))
                    {
                        imgFact.Load(inStream).Resize(size).Format(format).Save(outStream);
                    }

                    string final_path = "";
                    string save_path = "";
                    if (typeOwner == ImageTypeOwner.User)
                    {
                        save_path = HttpContext.Current.Server.MapPath(user_folder_path); //+ owner + ExtensionMethods.GetDescription(typeSize) +".png";
                        final_path = user_folder_path;
                    }
                    else if (typeOwner == ImageTypeOwner.Org)
                    {
                        save_path = HttpContext.Current.Server.MapPath(org_folder_path); //+ owner + ExtensionMethods.GetDescription(typeSize) + ".png";
                        final_path = org_folder_path;
                    }
                    else if (typeOwner == ImageTypeOwner.Product)
                    {
                        save_path = HttpContext.Current.Server.MapPath(product_folder_path);
                        final_path = product_folder_path;
                    }
                    else if (typeOwner == ImageTypeOwner.Topic)
                    {
                        save_path = HttpContext.Current.Server.MapPath(topic_folder_path);
                        final_path = topic_folder_path;
                    }
                    else
                    {
                        //Something went wrong
                    }

                    save_path += owner + ExtensionMethods.GetDescription(typeSize) + ".png";
                    final_path += owner + ExtensionMethods.GetDescription(typeSize) + ".png";

                    var final_img = System.IO.File.Create(save_path);
                    outStream.Seek(0, SeekOrigin.Begin);
                    outStream.CopyTo(final_img);
                    final_img.Close();

                    return final_path;//final_path.Replace(@"\", "/");
                }

            }
        }

        public void UserIconImageUpload(ApplicationUser user, HttpPostedFileBase img)
        {
            var UserImg = imageRepository.GetUserImage(user.Id);
            string tmpImg = TMPImageHandler(img, user.DisplayName);

            string finalImg = ImageConvert(tmpImg, user.DisplayName, ImageTypeSize.icon_100, ImageTypeOwner.User);

            UserImg.icon_100 = finalImg;
            UserImg.icon_20 = finalImg;
            imageRepository.Update(UserImg);
            unitOfWork.Commit();
        }

        public void OrgBannerImageUpload(Org org, HttpPostedFileBase img)
        {
            var OrgImg = imageRepository.GetOrgImage(org.Id);
            string tmpImg = TMPImageHandler(img, org.orgName);

            string finalImg = ImageConvert(tmpImg, org.orgName, ImageTypeSize.banner, ImageTypeOwner.Org);

            OrgImg.banner = finalImg;
            imageRepository.Update(OrgImg);
            unitOfWork.Commit();
        }

        public void OrgIcon20ImageUpload(Org org, HttpPostedFileBase img)
        {
            var OrgImg = imageRepository.GetOrgImage(org.Id);
            string tmpImg = TMPImageHandler(img, org.orgName);

            string finalImg = ImageConvert(tmpImg, org.orgName, ImageTypeSize.icon_20, ImageTypeOwner.Org);

            OrgImg.icon_20 = finalImg;
            imageRepository.Update(OrgImg);
            unitOfWork.Commit();
        }

        public void OrgIcon100ImageUpload(Org org, HttpPostedFileBase img)
        {
            var OrgImg = imageRepository.GetOrgImage(org.Id);
            string tmpImg = TMPImageHandler(img, org.orgName);

            string finalImg = ImageConvert(tmpImg, org.orgName, ImageTypeSize.icon_100, ImageTypeOwner.Org);

            OrgImg.icon_100 = finalImg;
            imageRepository.Update(OrgImg);
            unitOfWork.Commit();
        }

        public void ProductBannerImageUpload(Product product, HttpPostedFileBase img)
        {
            var ProductImg = imageRepository.GetProductImage(product.Id);
            string tmpImg = TMPImageHandler(img, product.ProductName);

            string finalImg = ImageConvert(tmpImg, product.ProductName, ImageTypeSize.banner, ImageTypeOwner.Product);

            ProductImg.banner = finalImg;
            imageRepository.Update(ProductImg);
            unitOfWork.Commit();
        }

        public void ProductIcon20ImageUpload(Product product, HttpPostedFileBase img)
        {
            var ProductImg = imageRepository.GetProductImage(product.Id);
            string tmpImg = TMPImageHandler(img, product.ProductName);

            string finalImg = ImageConvert(tmpImg, product.ProductName, ImageTypeSize.icon_20, ImageTypeOwner.Product);

            ProductImg.icon_20 = finalImg;
            imageRepository.Update(ProductImg);
            unitOfWork.Commit();
        }

        public void ProductIcon100ImageUpload(Product product, HttpPostedFileBase img)
        {
            var ProductImg = imageRepository.GetProductImage(product.Id);
            string tmpImg = TMPImageHandler(img, product.ProductName);

            string finalImg = ImageConvert(tmpImg, product.ProductName, ImageTypeSize.icon_100, ImageTypeOwner.Product);

            ProductImg.icon_100 = finalImg;
            imageRepository.Update(ProductImg);
            unitOfWork.Commit();
        }

        public void TopicBannerImageUpload(Topic topic, HttpPostedFileBase img)
        {
            var TopicImg = imageRepository.GetTopicImage(topic.Id);
            string tmpImg = TMPImageHandler(img, topic.topicName);

            string finalImg = ImageConvert(tmpImg, topic.topicName, ImageTypeSize.banner, ImageTypeOwner.Product);

            TopicImg.banner = finalImg;
            imageRepository.Update(TopicImg);
            unitOfWork.Commit();
        }

        public void TopicIcon20ImageUpload(Topic topic, HttpPostedFileBase img)
        {
            var TopicImg = imageRepository.GetTopicImage(topic.Id);
            string tmpImg = TMPImageHandler(img, topic.topicName);

            string finalImg = ImageConvert(tmpImg, topic.topicName, ImageTypeSize.icon_20, ImageTypeOwner.Product);

            TopicImg.icon_20 = finalImg;
            imageRepository.Update(TopicImg);
            unitOfWork.Commit();
        }

        public void TopicIcon100ImageUpload(Topic topic, HttpPostedFileBase img)
        {
            var TopicImg = imageRepository.GetTopicImage(topic.Id);
            string tmpImg = TMPImageHandler(img, topic.topicName);

            string finalImg = ImageConvert(tmpImg, topic.topicName, ImageTypeSize.icon_100, ImageTypeOwner.Product);

            TopicImg.icon_100 = finalImg;
            imageRepository.Update(TopicImg);
            unitOfWork.Commit();
        }
    }
}