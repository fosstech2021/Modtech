using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BasePackageModule1.Models.Menu;

namespace BasePackageModule2.Models.Menu
{
    public class Menu
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Url { get; set; }

        public int Order { get; set; }

        [DisplayName("Menu Type")]
        public string Type { get; set; }

        [DisplayName("Show On Home Screen?")]
        public bool ShowOnHomeScreen { get; set; }

        [DisplayName("Display as a")]
        public string DisplayAs { get; set; }

        [DisplayName("Menu Products")]
        public ICollection<MenuProduct> MenuProducts { get; set; }

        [DisplayName("Menu Categories")]
        public ICollection<MenuCategory>  MenuCategories { get; set; }
    }
}
