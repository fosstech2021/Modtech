using System.ComponentModel.DataAnnotations;

namespace BasePackageModule2.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Company { get; set; }

        [Required]
        [MinLength(5)]
        public string Message { get; set; }
    }
}