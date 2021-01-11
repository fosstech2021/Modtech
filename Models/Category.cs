using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BasePackageModule2.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
