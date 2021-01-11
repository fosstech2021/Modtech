using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BasePackageModule2.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            //var orders = await _context.Orders.Where(a => a.UserId == user.Id).ToListAsync();
            //var address = await _context.Addresses.Where(u => u.UserId == user.Id).ToListAsync();
            //_context.RemoveRange(address);
            //_context.RemoveRange(orders);
            //await _context.SaveChangesAsync();

            return View(await _context.Orders
                .Include(p => p.OrderProducts)
                .ThenInclude(p => p.Product)
                .ThenInclude(i => i.Images)
                //.Where(p => p.PaymentStatus == "Credit")
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(a => a.Id).ToListAsync());
        }
        
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
