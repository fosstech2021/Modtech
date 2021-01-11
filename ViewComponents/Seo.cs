using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.ViewComponents
{
    [ViewComponent(Name = "Seo")]
    public class Seo: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Seo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var courses = new Models.Seo();
            if (_context.Seo.Any())
            {
                courses = await _context.Seo.FirstOrDefaultAsync();
            }
          
          
            return View("Index", courses);
        }
    }
}
