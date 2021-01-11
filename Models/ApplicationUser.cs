using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BasePackageModule2.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        public virtual IEnumerable<Address> Addresses { get; set; }

        public virtual IEnumerable<Order> Orders { get; set; }

        public virtual IEnumerable<WishList> WishList { get; set; }

        public virtual IEnumerable<Cart> Cart { get; set; }
    }

}
