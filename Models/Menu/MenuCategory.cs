using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Models;

namespace BasePackageModule1.Models.Menu
{
    public class MenuCategory
    {
        public int Id { get; set; }

        public BasePackageModule2.Models.Menu.Menu Menu { get; set; }
        public int MenuId { get; set; }

        public Category Category { get; set; }

        public int CategoryId { get; set; }
    }
}
