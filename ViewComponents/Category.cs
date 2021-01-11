using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasePackageModule2.Data;

namespace BasePackageModule2.ViewComponents
{
    [ViewComponent(Name = "Category")]
    public class Category : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Category(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Models.Category> categories = await _context.Categories
                .Include(c => c.SubCategories)
                .ThenInclude(p => p.Products)
                .Include(p => p.Products)
                .ToListAsync();
            return View("Index", categories);
        }
    }
}