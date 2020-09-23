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
        /// GET /admin/products
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var products = await this.context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        /// <summary>
        /// GET /admin/products/details/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to show</param>
        public async Task<IActionResult> Details(int id)
        {
            var product = await this.context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }

        /// <summary>
        /// GET /admin/products/create
        /// </summary>
        public IActionResult Create()
        {
            this.ViewBag.CategoryId = new SelectList(this.context.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            return View();
        }

        /// <summary>
        /// POST /admin/products/create 
        /// <br/>
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="product">Form data used to create the record</param>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Create(Product product)
        {
            this.ViewData["CategoryId"] = new SelectList(this.context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);

            if (this.ModelState.IsValid) {

                if (product.CategoryId == 0) {
                    this.ModelState.AddModelError("CategoryId", "The Category field is required");
                    return View(product);
                }

                product.Slug = product.Name.ToLower().Replace(" ", "-");
                var slug = await this.context.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null) {
                    this.ModelState.AddModelError("", "A Product with this name already exists");
                    return View(product);
                }

                string imageName = "noimage.png";
                if (product.ImageUpload != null) {
                    string uploadsDir = Path.Combine(this.webHostEnvironment.WebRootPath, "media", "products");
                    imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
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
        /// GET /admin/products/edit/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to edit</param>
        public async Task<IActionResult> Edit(int id)
        {
            var product = await this.context.Products.FindAsync(id);
            if (product == null) {
                this.TempData["Error"] = "You can not edit that which does not exist <br/> - the developer";
                return RedirectToAction("Index");
            }
            this.ViewData["CategoryId"] = new SelectList(this.context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);
            return View(product);
        }

        /// <summary>
        /// POST /admin/products/edit/{id}
        /// <br/>
        /// To protect from overposting attacks, enable the specific properties you want to bind to, for more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id">Primary key of the record to edit</param>
        /// <param name="product">Form data used to edit the record</param>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            this.ViewData["CategoryId"] = new SelectList(this.context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);

            if (this.ModelState.IsValid) {

                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await this.context.Products.Where(x => x.Id != id).FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null) {
                    this.ModelState.AddModelError("", "A Product with this name already exists");
                    return View(product);
                }

                if (product.ImageUpload != null) {
                    string uploadsDir = Path.Combine(this.webHostEnvironment.WebRootPath, "media", "products");

                    if (!string.Equals(product.Image, "noimage.png")) {
                        string oldImagePath = Path.Combine(uploadsDir, product.Image);
                        if (System.IO.File.Exists(oldImagePath)) {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
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
        /// GET /admin/products/delete/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to delete</param>
        public async Task<IActionResult> Delete(int id)
        {
            var product = await this.context.Products.FindAsync(id);
            if (product == null) {
                this.TempData["Error"] = "You can not delete that which does not exist <br/> - the developer";
            } else {
                if (!string.Equals(product.Image, "noimage.png")) {
                    string uploadsDir   = Path.Combine(this.webHostEnvironment.WebRootPath, "media", "products");
                    string oldImagePath = Path.Combine(uploadsDir, product.Image);
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

        private string UploadsDir() 
        {
            return Path.Combine(this.webHostEnvironment.WebRootPath, "media", "products");
        }

        private bool ProductExists(int id)
        {
            return this.context.Products.Any(p => p.Id == id);
        }

        // private bool CategoryExists(int id)
        // {
        //     return this.context.Products.Any(p => p.Id == id);
        // }

    }
}
