using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Extensions;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Areas.TomBase.Controllers
{
    [Area("TomBase")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class SliderImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SliderImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SliderImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.SliderImages.ToListAsync());
        }

        // POST: Admin/SliderImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Create(List<IFormFile> images)
        {
            if (images == null || images.Count <= 0) return RedirectToAction(nameof(Index));

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/img/slider"));
            }

            foreach (IFormFile item in images)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider", imageName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    item.CopyTo(stream);
                }

                var image = new SliderImage { Image = $"/img/slider/{imageName}" };

                _context.Add(image);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index)).WithSuccess("Slider image has been added.", null);
        }


        // GET: Admin/SliderImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sliderImage = await _context.SliderImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sliderImage == null)
            {
                return NotFound();
            }

            return View(sliderImage);
        }

        // POST: Admin/SliderImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sliderImage = await _context.SliderImages.FindAsync(id);
            _context.SliderImages.Remove(sliderImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)).WithSuccess("Slider image has been deleted.", null);
        }

        private bool SliderImageExists(int id)
        {
            return _context.SliderImages.Any(e => e.Id == id);
        }
    }
}
