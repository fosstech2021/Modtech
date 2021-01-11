using System.Collections.Generic;
using BasePackageModule2.Models;
using BasePackageModule2.Models.Menu;

namespace BasePackageModule2.ViewModels
{
    public class NavViewModel
    {
        public List<Page> More { get; set; }
        public Logo Logo { get; set; }

        public BusinessProfile BusinessProfile { get; set; }

        public List<Category> Categories { get; set; }

        public List<Product> Products { get; set; }

        public List<Menu> Menus { get; set; }
    }
}
