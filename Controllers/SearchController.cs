using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using X.PagedList;

namespace BasePackageModule2.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET
        public async Task<IActionResult> Index(string category, string query, int minPrice, int maxPrice, int pageindex = 1, int PageSize = 6, string orderby = null, int colorId = 0)
        {
          
            IQueryable<Product> products = from s in _context.Products
                                           select s;

            if (category != null)
            {
                products = products.Where(c => c.Category.Name == category);
            }

            if (query != null)
            {
                products = products.Where(s => s.Name.Contains(query));
            }

            switch (orderby)
            {
                case "Date":
                    products = products.OrderBy(s => s.CreatedAt);
                    break;
                case "price":
                    products = products.OrderBy(s => s.FinalPrice);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(s => s.FinalPrice);
                    break;
                default:
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() { Value="6", Text= "6" },
                new SelectListItem() { Value="12", Text= "12" },
                new SelectListItem() { Value="24", Text= "24" },
                new SelectListItem() { Value="48", Text= "48" },
                new SelectListItem() { Value="100", Text= "100" },
            };
            if (minPrice > 0)
            {
                products = products.Where(p => p.FinalPrice >= minPrice);
                ViewBag.maxPrice = minPrice;
            }

            if (maxPrice > 0)
            {
                products = products.Where(p => p.FinalPrice <= maxPrice);
                ViewBag.minPrice = maxPrice;
            }


            ViewBag.categoryName = category;
            ViewBag.orderby = orderby;
            ViewBag.query = query;

            return View(
                await products
                    .Include(p => p.Images)
                    .ToPagedListAsync(pageNumber: pageindex, pageSize: PageSize));
        }
    }
}