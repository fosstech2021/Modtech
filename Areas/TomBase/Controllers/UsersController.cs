using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BasePackageModule2.Areas.TomBase.ViewModels;
using BasePackageModule2.Data;
using BasePackageModule2.Extensions;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Areas.TomBase.Controllers
{
    [Area("TomBase")]
    [Authorize(Roles = "SuperAdmin,Admin")]

   // [Authorize(Roles = "SuperAdmin,Admin")]
    public class UsersController : Controller
    {
      
            private readonly ApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;


            public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                _context = context;
                _userManager = userManager;
                _roleManager = roleManager;
            }

            // GET: Users
            public IActionResult Index()
            {
                return View(_userManager.Users.AsEnumerable());
            }

            // GET: Users/Details/5
            public async Task<IActionResult> Details(string? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }

            // GET: Users/Create
            public ActionResult Create()
            {
                ViewData["Roles"] = new SelectList(_roleManager.Roles, "Name", "Name");

            return View();
            }

            // POST: Users/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(RegisterViewModel Input)
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        PhoneNumber = Input.MobileNumber
                    };
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRolesAsync(user, roles: Input.Roles);

                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index)).WithSuccess("User has been added.", null);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                ViewData["Roles"] = new SelectList(_roleManager.Roles, "Name", "Name");

            return View(Input).WithError("Please fill the correct details.", null);
            }

            // GET: Users/Edit/5
            public async Task<IActionResult> Edit(string? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                UpdateUser updateUser = new UpdateUser
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobileNumber = user.PhoneNumber
                };

                ViewData["Roles"] = new MultiSelectList(_roleManager.Roles, "Name", "Name", _userManager.GetRolesAsync(user).Result.ToArray());

            return View(updateUser);
            }

            // POST: Users/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(string id, UpdateUser user)
            {
                if (id != user.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var AUser = await _userManager.FindByIdAsync(id);
                        AUser.FirstName = user.FirstName;
                        AUser.LastName = user.LastName;
                        AUser.Email = user.Email;
                        AUser.UserName = user.Email;
                        AUser.PhoneNumber = user.MobileNumber;

                        await _userManager.UpdateAsync(AUser);

                        if (user.Password != null)
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(AUser);

                            await _userManager.ResetPasswordAsync(AUser, token, user.Password);
                        }

                        await _userManager.RemoveFromRolesAsync(AUser, await _roleManager.Roles.Select(a => a.Name).ToListAsync());

                        await _userManager.AddToRolesAsync(AUser, user.Roles);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index)).WithSuccess("User has been updated.", null);
                }

                return View(user).WithError("Please fill the correct details.", null);
            }

            // GET: Users/Delete/5
            public async Task<IActionResult> Delete(string? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }

            // POST: Users/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete(string id, IFormCollection collection)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id.ToString());
                    await _userManager.DeleteAsync(user);
                    return RedirectToAction(nameof(Index)).WithSuccess("User has been removed.", null);
                }
                catch
                {
                    return View().WithError("Something went wrong.", null);
                }
            }

            private bool UserExists(string id)
            {
                return _context.Users.Any(e => e.Id == id);
            }
        }
    }