using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using BasePackageModule2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.ViewComponents
{
    [ViewComponent(Name = "Favicon")]
    public class Favicon:ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Favicon(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var logo = new Logo();

            if(_context.Logos.Any())
            {
             logo = await _context.Logos.FirstOrDefaultAsync();
            }
         

            NavViewModel model = new NavViewModel
            {

                

                Logo = logo,


            };

            return View("Index", model);
        }
    }
}
