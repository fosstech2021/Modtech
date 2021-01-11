using System.ComponentModel.DataAnnotations;

namespace BasePackageModule2.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Required]
        [Display(Name = "Pin Code")]
        public int PinCode { get; set; }
        public string Locality { get; set; }
        [Required]
        [Display(Name = "Address (Area & Street)")]
        public string MainAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Landmark { get; set; }
        [Display(Name = "Alternate Phone")]
        public string AlternatePhone { get; set; }
    }
}