using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMSShoppingCart.Infrastructure;
using CMSShoppingCart.Models;

namespace CMSShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly CMSShoppingCartContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController(CMSShoppingCartContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     GET /admin/products
        /// </summary>
        public async Task<IActionResult> Index(int p = 1)
        {
            var pageSize = 5;
            var count = context.Products.Count();
            var products = await this.context.Products
                                             .OrderByDescending(x => x.Id)
                                             .Include(x => x.Category)
                                             .Skip((p - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToListAsync();
            ViewBag.PageNumber = p;
            ViewBag.PageRange  = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)count / pageSize);
            return View(products);
        }

        /// <summary>
        ///     GET /admin/products/details/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to show</param>
        public async Task<IActionResult> Details(int id)
        {
            var product = await this.context.Products
                                            .Include(x => x.Category)
                                            .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }

        /// <summary>
        ///     GET /admin/products/create
        /// </summary>
        public IActionResult Create()
        {
            // this.ViewBag.CategoryId = new SelectList(this.context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            AddCategoryIdToViewData();
            return View();
        }

        /// <summary>
        ///     POST /admin/products/create
        /// </summary>
        /// <param name="product">Form data used to create the record</param>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Create(Product product)
        {
            AddCategoryIdToViewData(product);

            if (this.ModelState.IsValid) {

                if (product.CategoryId == 0) {
                    this.ModelState.AddModelError("CategoryId", "The Category field is required");
                    return View(product);
                }

                if (! await AddSlug(product)) {
                    return View(product);
                }

                string imageName = "noimage.png";
                if (product.ImageUpload != null) {
                    imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(UploadsDir(), imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }
                product.Image = imageName;

                this.context.Add(product);
                await this.context.SaveChangesAsync();
                this.TempData["Success"] = $"New product '{product.Name}' has been added";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        /// <summary>
        ///     GET /admin/products/edit/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to edit</param>
        public async Task<IActionResult> Edit(int id)
        {
            var product = await this.context.Products.FindAsync(id);
            if (product == null) {
                this.TempData["Error"] = "You can not edit that which does not exist <br/> - the developer";
                return RedirectToAction("Index");
            }
            AddCategoryIdToViewData(product);
            return View(product);
        }

        /// <summary>
        ///     POST /admin/products/edit/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to edit</param>
        /// <param name="product">Form data used to edit the record</param>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            AddCategoryIdToViewData(product);

            if (this.ModelState.IsValid) {

                if (! await AddSlug(product)) {
                    return View(product);
                }

                if (product.ImageUpload != null) {
                    if (!string.Equals(product.Image, "noimage.png")) {
                        string oldImagePath = Path.Combine(UploadsDir(), product.Image);
                        if (System.IO.File.Exists(oldImagePath)) {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(UploadsDir(), imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                try {
                    this.context.Update(product);
                    await this.context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!ProductExists(product.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                this.TempData["Success"] = $"Product '{product.Name}' has been updated";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        /// <summary>
        ///     GET /admin/products/delete/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to delete</param>
        public async Task<IActionResult> Delete(int id)
        {
            var product = await this.context.Products.FindAsync(id);
            if (product == null) {
                this.TempData["Error"] = "You can not delete that which does not exist <br/> - the developer";
            } else {
                if (!string.Equals(product.Image, "noimage.png")) {
                    string oldImagePath = Path.Combine(UploadsDir(), product.Image);
                    if (System.IO.File.Exists(oldImagePath)) {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                this.context.Products.Remove(product);
                await this.context.SaveChangesAsync();
                this.TempData["Success"] = $"Product '{product.Name}' has been deleted";
            }
            return RedirectToAction("Index");
        }

        // =====================================================================
        // Private Helper Methods
        // =====================================================================

        /// <summary>
        ///     Returns a path for storing uploaded images.
        /// </summary>
        private string UploadsDir()
        {
            return Path.Combine(this.webHostEnvironment.WebRootPath, "media", "products");
        }

        private bool ProductExists(int id)
        {
            return this.context.Products.Any(x => x.Id == id);
        }

        private void AddCategoryIdToViewData(Product product = null)
        {
            if (!(product == null)) {
                this.ViewData["CategoryId"] = new SelectList(this.context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);
            } else {
                this.ViewData["CategoryId"] = new SelectList(this.context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            }
        }

        /// <summary>
        ///     Generate a <see cref="Product.Slug"/> and check to see if it exists in the database yet.
        /// </summary>
        /// <param name="product">The <see cref="Product"/> for which a <c>Slug</c> should be generated</param>
        /// <returns><c>true</c> if the <c>Slug</c> was successfully added</returns>
        /// <returns><c>false</c> in all other cases</returns>
        private async Task<bool> AddSlug(Product product)
        {
            product.Slug = product.Name.ToLower().Replace(" ", "-");

            var slug = await this.context.Products
                                         .Where(x => x.Id != product.Id)
                                         .FirstOrDefaultAsync(x => x.Slug == product.Slug);

            if (slug != null) {
                this.ModelState.AddModelError("", "A Product with this name already exists");
                return false;
            } else {
                return true;
            }
        }

    }
}
