using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasePackageModule2.Models.Menu
{
    public class MenuProduct
    {
        public int Id { get; set; }

        public Menu Menu { get; set; }
        public int MenuId { get; set; }

        public Product Product { get; set; }

        public int ProductId { get; set; }
    }
}
