using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Sigil.Models
{
    public class OrgRegisterViewModel
    {
        [Required]
        [StringLength( 16, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5 )]
        [Display( Name = "Organization Name" )]
        public string orgName { get; set; }

        [Required]
        [StringLength( 16, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5 )]
        [Display( Name = "Requested Organization URL for Sigil" )]
        public string orgURL { get; set; }

        [Required]
        [StringLength(256)]
        [Display( Name = "Organization's Website")]
        public string orgWebsite { get; set; }
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Organization's Phone Number")]
        public string orgContact { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "About your Organization")]
        public string orgComment { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Your Name")]
        public string orgAdminName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Confirm")]
        [Compare("Email", ErrorMessage = "The Email and confirmation email do not match.")]
        public string Email_confirm { get; set; }

        [Required]
        [StringLength(16, ErrorMessage ="The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [Display(Name = "Username")]
        public string DisplayName { get; set; }

    }

    public class Org
    {
        [Key]
        public int Id { get; set; }
        public string orgName { get; set; }
        public string orgURL { get; set; }
        public int topicid { get; set; }
        public int viewCount { get; set; }
        public string website { get; set; }
        public DateTime lastView { get; set; }
        public XElement UserID { get; set; }

        public List<Issue> OrgIssues { get; set; }
        public List<Category> OrgCategories { get; set; }

    }
}
