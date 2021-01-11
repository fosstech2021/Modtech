using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasePackageModule1.Areas.Identity.Pages.Account.Manage
{
    public class EditAddressModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditAddressModel( ApplicationDbContext context)
        {
            _context = context;
          }
        [BindProperty]
        public Address Address { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id!=null)
            {
                Address = await _context.Addresses.FindAsync(id);
            }
            return Page();
        }

        public async Task<ActionResult> OnPostAsync()
        {
            var address = Address;
            if(!ModelState.IsValid)
            {
                return Page();
            }
            _context.Entry(address).Property(x => x.Name).IsModified = true;
            _context.Entry(address).Property(x => x.MobileNumber).IsModified = true;
            _context.Entry(address).Property(x => x.PinCode).IsModified = true;
            _context.Entry(address).Property(x => x.Locality).IsModified = true;
            _context.Entry(address).Property(x => x.MainAddress).IsModified = true;
            _context.Entry(address).Property(x => x.City).IsModified = true;
            _context.Entry(address).Property(x => x.State).IsModified = true;
            _context.Entry(address).Property(x => x.Landmark).IsModified = true;
            _context.Entry(address).Property(x => x.AlternatePhone).IsModified = true;
            _context.Update(address);
           await   _context.SaveChangesAsync();
            return RedirectToPage("Address");
        }
    }
}
