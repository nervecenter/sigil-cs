using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Sigil.Controllers;
using System.Web;

namespace Sigil.Models
{
    public enum ImageType
    {
        User, Org, Product, Topic
    }

    public static class ImageFilePaths
    {
        public static string default_folder_path = "/Images/Default/";
    }

    public class Image
    {
        [Key]
        public int Id { get; set; }


        public string OwnerId { get; set; }

        /// <summary>
        /// Of type INT but set by using ENUM ImageType
        /// </summary>
        public int imgType { get; set; }

        public string icon_20 { get; set; }
        public string icon_100 { get; set; }

        public bool ShowBanner { get; set; }
        public string banner { get; set; }

        public Image()
        {
            int defaultIMG = RNG.RandomNumber(1, 6);

            icon_100 = ImageFilePaths.default_folder_path + "default" + defaultIMG + ".png";
            icon_20 = ImageFilePaths.default_folder_path + "default20.png";
            ShowBanner = true;
            banner = ImageFilePaths.default_folder_path + "defaultBanner.png";
        }

    }
}