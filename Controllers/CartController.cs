using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Extensions;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var  cart = await _context.Carts.Include(p => p.Product).ThenInclude(i => i.Images).Where(u => u.UserId == user.Id).ToListAsync();
            return View(cart);
        }

        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var cart = await _context.Carts.Where(u => u.UserId == user.Id).Where(i => i.Id == id).FirstAsync();

                if (cart != null)
                {
                    _context.Remove(cart);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index)).WithSuccess("Item has been removed form cart.", null);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCart([FromForm] List<CartItem> items)
        {
            foreach (var cartItem in items)
            {
                var cart = await _context.Carts.FirstAsync(i => i.Id == cartItem.cartId);
                if (cart != null)
                {
                    cart.Qty = cartItem.QTY;
                }
            }

            await _context.SaveChangesAsync();

            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int productId, int qty = 1)
        {

            Product product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var exist = await _context.Carts.Where(p => p.ProductId == productId).Where(u => u.UserId == user.Id)
                .FirstOrDefaultAsync();
            if (exist != null)
            {
                return RedirectToAction(nameof(Index)).WithWarning("Product already is in cart.", null);
            }

            if (product != null)
            {
                var cart = new Cart
                {
                    ProductId = product.Id,
                    UserId = user.Id,
                    Qty = qty
                };

                _context.Add(cart);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index)).WithSuccess("Product added in cart", null);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RemoveShop(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var cart = await _context.Carts.Where(u => u.UserId == user.Id).Where(i => i.Id == id).FirstAsync();

                if (cart != null)
                {
                    _context.Remove(cart);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index)).WithSuccess("Item has been removed form cart.", null);
                }
            }
            return NotFound();
        }

    }

    public class CartItem
    {
        public int cartId { get; set; }
        public int QTY { get; set; }
    }
}