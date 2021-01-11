using System.Collections.Generic;
using BasePackageModule2.Models;

namespace BasePackageModule2.ViewModels
{
    public class ItemViewModel
    {
        public Product Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }
    }
}
