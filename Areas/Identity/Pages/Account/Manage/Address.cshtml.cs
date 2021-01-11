using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;

namespace BasePackageModule1.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AddressModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Address> Addresses { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            Addresses = await _context.Addresses.Where(u => u.UserId == user.Id).ToListAsync();

            return Page();
        }
        public async Task<ActionResult> OnGetDeleteAsync(int? id)
        {
            if (id != null)
            {
                var add = await _context.Addresses.FindAsync(id);

                _context.Remove(add);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("Address");

        }
    }

}