using System.Threading.Tasks;
using BasePackageModule2.Areas.TomBase.ViewModels;
using BasePackageModule2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Areas.TomBase.Controllers
{
    [Area("TomBase")]
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TomBaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TomBaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminViewModel
            {
                Posts = await _context.Posts.CountAsync(),
                Messages = await _context.ContactMessages.CountAsync(),
                Categories = await _context.Categories.CountAsync(),
                Products = await _context.Products.CountAsync()
            };

            return View(model);
        }
    }
}