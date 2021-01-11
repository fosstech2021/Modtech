using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MailKit;

namespace BasePackageModule2.Models
{
    public class BusinessProfile
    {
        public int Id { get; set; }

        [DisplayName("Business Name")]
        [Required]
        public string BusinessName{ get; set; }
        public string Title { get; set; }

        [DisplayName("Email Address")]
        [Required]
        public string EmailAddress { get; set; }

        [DisplayName("Registered Contact Number")]
        [Required]
        public string RegisteredContactNumber{ get; set; }

        [DisplayName("Alternate Contact Number")]
        public string AlternateContactNumber { get; set; }

        [DisplayName("Display Contact 1")] 
        public string DisplayContact1 { get; set; }

        [DisplayName("Display Contact 2")]
        public string DisplayContact2 { get; set; }
        
        [DisplayName("Display Contact 3")]
        public string DisplayContact3 { get; set; }

        [DisplayName("Address")]
        [Required]
        public string Address { get; set; }

        [DisplayName("City")]
        [Required]
        public string City { get; set; }

        [Required]
        public string State{ get; set; }

        [DisplayName("Country")] 
        [Required]
        public string Country { get; set; }

       [Required]
        public string Pincode { get; set; }


        [DisplayName("WhatsApp for Business Number")]
        public string WhatsAppForBusinessNumber { get; set; }

        [DisplayName("Facebook Page URL")]
        public string FacebookPageURL { get; set; }

        [DisplayName("Instagram Page URL")]
        public string InstagramPageURL { get; set; }

        [DisplayName("Twitter URL")]
        public string TwitterURL { get; set; }

        [DisplayName("LinkedIn URL")]
        public string LinkedInURL { get; set; }

        [DisplayName("Youtube URL")]
        public string YoutubeURL { get; set; }

        [DisplayName("Other Website")]
        public string OtherWebsite { get; set; }

        public string Copyright { get; set; }
    }
}
