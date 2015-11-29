using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public enum ImageType
    {
        User, Org, Product, Topic
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
        public string banner { get; set; }

    }
}