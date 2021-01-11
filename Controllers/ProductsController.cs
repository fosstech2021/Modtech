using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BasePackageModule2.Utility;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using BasePackageModule2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BasePackageModule2.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string category, string query, int minPrice, int maxPrice, int pageindex = 1, int PageSize = 6, string orderby = null)
        {

            IQueryable<Product> products = from s in _context.Products
                                           select s;

            products = products.Where(s => s.Status == true);

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
                //case "Date":
                //    products = products.OrderBy(s => s.CreatedAt);
                //    break;
                case "price":
                    products = products.OrderBy(s => s.FinalPrice);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(s => s.FinalPrice);
                    break;
                default:
                    products = products.OrderBy(s => s.SortOrder);
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

          
            ViewBag.orderby = orderby;
            ViewBag.category = category;

            return View(
                await products
                    .ToPagedListAsync(pageNumber: pageindex, pageSize: PageSize));


        }

        [HttpGet]
        [Route("search/{min}/{max}")]
        public IActionResult Search( double min, double max)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Item =  _context.Products.Include(m => m.Category).Include(m => m.SubCategory).ToList(),
                Category =  _context.Categories.ToList(),
                Coupon = _context.Coupons.Where(c => c.IsActive == true).ToList()
            };
            return new JsonResult(productViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            if (!ProductExists(id))
            {
                return NotFound();
            }
            var product = await _context.Products
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .Include(c => c.Images)
                .Where(m => m.Id == id).FirstOrDefaultAsync();
            List<Product> fundList = _context.Products.Where(p=>p.Id!=id).OrderBy(o=>o.SortOrder).ToList();
            ViewBag.Funds = fundList;

            return View(product);
        }


        [Route("Show/{Id?}/{menuName?}")]
        public async Task<IActionResult> Show(int? id, string? menuName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.Include(a => a.MenuProducts).ThenInclude(a => a.Product).FirstOrDefaultAsync(a => a.Id == id);
            if (menu != null)
            {
                var products = await menu.MenuProducts.Where(a => a.MenuId == menu.Id).Select(a => a.Product).ToListAsync();
                return View(products);
            }

            return NotFound();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}