using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Areas.TomBase.Controllers
{
    [Area("TomBase")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category).Include(p => p.SubCategory).OrderBy(a => a.SortOrder);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(i => i.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile mainImage, List<IFormFile> images, Product product)
        {
            if (ModelState.IsValid)
            {
                if (mainImage == null)
                {
                    ModelState.AddModelError("Image", "Product Image is required.");
                    return View(product);
                }
                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/img/products"));
                }
                var mainImageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);

                //Get url To Save
                var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", mainImageName);

                await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                {
                    await mainImage.CopyToAsync(stream);
                }

                product.Image = $"/img/products/{mainImageName}";

                _context.Add(product);
                await _context.SaveChangesAsync();

                if (images != null)
                {
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products")))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                            "wwwroot/img/products"));
                    }

                    foreach (IFormFile item in images)
                    {
                        //Set Key Name
                        var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                        //Get url To Save
                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", imageName);

                        await using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }

                        var image = new ProductImage()
                        {
                            ProductId = product.Id,
                            Img = $"/img/products/{imageName}"
                        };

                        _context.Add(image);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Name", product.SubCategoryId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Images ).FirstOrDefaultAsync(i => i.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Name", product.SubCategoryId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile mainImage, List<IFormFile> images, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thisProduct = await _context.Products.FirstOrDefaultAsync(i => i.Id == product.Id);
                    if (thisProduct == null)
                    {
                        return NotFound();
                    }

                    if (mainImage != null)
                    {
                        var mainImageName = Guid.NewGuid() + Path.GetExtension(mainImage.FileName);

                        //Get url To Save
                        var mainImageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", mainImageName);

                        await using (var stream = new FileStream(mainImageSavePath, FileMode.Create))
                        {
                            await mainImage.CopyToAsync(stream);
                        }

                        thisProduct.Image = $"/img/products/{mainImageName}";
                    }

                    if (images != null)
                    {
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                "wwwroot/img/products"));
                        }

                        foreach (IFormFile item in images)
                        {
                            //Set Key Name
                            var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                            //Get url To Save
                            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/products", imageName);

                            await using (var stream = new FileStream(savePath, FileMode.Create))
                            {
                                await item.CopyToAsync(stream);
                            }

                            var image = new ProductImage()
                            {
                                ProductId = product.Id,
                                Img = $"/img/products/{imageName}"
                            };

                            _context.Add(image);
                        }
                    }

                    thisProduct.Name = product.Name;
                    thisProduct.CategoryId = product.CategoryId;
                    thisProduct.Description = product.Description;
                    thisProduct.SubCategoryId = product.SubCategoryId;
                    thisProduct.BasePrice = product.BasePrice;
                    thisProduct.DiscountAmount = product.DiscountAmount;
                    thisProduct.FreeShipping = product.FreeShipping;
                    thisProduct.Status = product.Status;
                    thisProduct.ProductTags = product.ProductTags;
                    thisProduct.ShortDescription = product.ShortDescription;
                    thisProduct.SortOrder = product.SortOrder;

                    _context.Update(thisProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "Name", product.SubCategoryId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(i => i.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


        public async Task<IActionResult> DeleteProductImage(int imageId, int productId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = productId });
        }
    }
}
