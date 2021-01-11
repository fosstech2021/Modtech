using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.ViewComponents
{
    [ViewComponent(Name = "TopBar")]
    public class TopBar : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public TopBar(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            if(_context.BusinessProfile.Any())
            {
                var topbars = await _context.BusinessProfile.FirstOrDefaultAsync();
                return View("Index", topbars);
            }
            return View("Index", new Models.BusinessProfile());
        }
    }
}