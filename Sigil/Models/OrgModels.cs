using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Sigil.Models
{

    public class Org
    {
        [Key]
        public int Id { get; set; }

        public string orgName { get; set; }
        public string orgURL { get; set; }

        public int? ImageId { get; set; }
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }

        public int viewCount { get; set; }
        public string website { get; set; }
        public DateTime lastView { get; set; }

        /// <summary>
        /// Is an xml string of UserIds
        /// </summary>
        public string UserID { get; set; }




        //public virtual List<Issue> Issues { get; set; }
        public virtual List<Product> Products { get; set; }

        public Org()
        {
            Products = new List<Product>();
        }

    }

    public class OrgApp
    {
        [Key]
        public int Id { get; set; }
        //Org info
        public string orgName { get; set; }
        public string orgURL { get; set; }
        public string website { get; set; }
        public string comment { get; set; }

        //Org Admin info

        public string DisplayName { get; set; }
        public string AdminEmail { get; set; }
        public string ContactNumber { get; set; }
        public string AdminContactName { get; set; }

        public bool OrgApproved { get; set; }
        public DateTime ApplyDate { get; set; }

        public OrgApp()
        {
            OrgApproved = false;
            ApplyDate = DateTime.UtcNow;
        }
    }
}
