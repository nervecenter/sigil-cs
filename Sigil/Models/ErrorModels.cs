using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sigil.Models
{


    public class Error
    {
        public int Id { get; set; }
        public DateTime error_date { get; set; }
        public string error_object { get; set; }
        public string error_exception { get; set; }
        public int error_lvl { get; set; }
    }
}