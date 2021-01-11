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
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(
            int id, 
            int minPrice, 
            int maxPrice, 
            int pageindex = 1, 
            int PageSize = 6, 
            string orderby = null, 
            int colorId = 0)
        {   
            if (!CategoryExists(id))
            {
                return NotFound();
            }

            

            IQueryable<Product> products = from s in _context.Products
                select s;

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
                    products= products.OrderBy(s => s.Name);
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
                ViewBag.minPrice = minPrice;
            }

            if (maxPrice > 0)
            {
                products = products.Where(p => p.FinalPrice <= maxPrice);
                ViewBag.maxPrice = maxPrice;
            }
          
            ViewBag.orderby = orderby;
            ViewBag.PerPageItems = PageSize;
            ViewBag.Category = await _context.SubCategories.FindAsync(id);
            return View(
                await products
                    .Where(c => c.SubCategoryId == id)
                    .Include(p => p.Images)
                    .ToPagedListAsync(pageNumber: pageindex, pageSize: PageSize));
        }

        private bool CategoryExists(int id)
        {
            return _context.SubCategories.Any(e => e.Id == id);
        }
    }
}
