using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasePackageModule1.Areas.Identity.Pages.Account.Manage
{
    public class AddNewAddressModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AddNewAddressModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [BindProperty]
        public Address Address { get; set; }
        public IActionResult OnGet()
        {

            return Page();
        }

        public async Task<ActionResult> OnPostAsync()
        {
            var address = Address;
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            address.UserId = user.Id;
            _context.Add(address);
            await _context.SaveChangesAsync();
            return RedirectToPage("Address");
        }
    }
}
