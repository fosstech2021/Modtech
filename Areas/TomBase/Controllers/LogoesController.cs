using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Areas.TomBase.Controllers
{
    [Area("TomBase")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class LogoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Logoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Logos.ToListAsync());
        }

        // GET: Admin/Logoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logo = await _context.Logos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logo == null)
            {
                return NotFound();
            }

            return View(logo);
        }

        // GET: Admin/Logoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Logoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile header, IFormFile footer, IFormFile favicon, Logo post)
        {
            if (ModelState.IsValid)
            {
                if (header == null)
                {
                    ModelState.AddModelError("Image", "Header Image is required.");
                    return View(post);
                }
                if (footer == null)
                {
                    ModelState.AddModelError("Image", "Footer Image is required.");
                    return View(post);
                }
                if (favicon == null)
                {
                    ModelState.AddModelError("Image", "Favcion Image is required.");
                    return View(post);
                }
                var HeaderImageName = Guid.NewGuid() + Path.GetExtension(header.FileName);
                var FooterImageName = Guid.NewGuid() + Path.GetExtension(footer.FileName);
                var FavImageName = Guid.NewGuid() + Path.GetExtension(favicon.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos"));
                }

                //Get url To Save
                var HeaderSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", HeaderImageName);
                var FooterSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", FooterImageName);
                var FaviconSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", FavImageName);

                await using (var stream = new FileStream(HeaderSavePath, FileMode.Create))
                {
                    header.CopyTo(stream);
                }
                await using (var stream = new FileStream(FooterSavePath, FileMode.Create))
                {
                    footer.CopyTo(stream);
                }
                await using (var stream = new FileStream(FaviconSavePath, FileMode.Create))
                {
                    favicon.CopyTo(stream);
                }

                post.HeaderLogo = $"/img/Logos/{HeaderImageName}";
                post.FooterLogo = $"/img/Logos/{FooterImageName}";
                post.Favicon = $"/img/Logos/{FavImageName}";


                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Admin/Logoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logo = await _context.Logos.FindAsync(id);
            if (logo == null)
            {
                return NotFound();
            }
            return View(logo);
        }

        // POST: Admin/Logoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Edit(int id, IFormFile image, Logo post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            var thisPost = await _context.Logos.FirstAsync(p => p.Id == post.Id);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(post);

            if (image != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);
              

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logos", imageName);
            

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                   
                }

                thisPost.HeaderLogo = $"/img/Posts/{imageName}";
                thisPost.FooterLogo = $"/img/Posts/{image}";
                thisPost.Favicon = $"/img/Logos/{image}";

            }
            try
            {
              
                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogoExists(post.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Logoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logo = await _context.Logos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (logo == null)
            {
                return NotFound();
            }

            return View(logo);
        }

        // POST: Admin/Logoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var logo = await _context.Logos.FindAsync(id);
            _context.Logos.Remove(logo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogoExists(int id)
        {
            return _context.Logos.Any(e => e.Id == id);
        }
    }
}
