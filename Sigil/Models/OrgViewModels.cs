using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        
        // orgTallBanner

        // orgSmallBanner

        // orgLargeIcon

        // orgSmallIcon

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
        public string UserName { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
