using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sigil.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        public string icon_20 { get; set; }
        public string icon_100 { get; set; }
        public string banner { get; set; }

    }
}