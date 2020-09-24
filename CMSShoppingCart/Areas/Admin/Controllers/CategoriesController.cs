using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMSShoppingCart.Infrastructure;
using CMSShoppingCart.Models;

namespace CMSShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly CMSShoppingCartContext context;

        public CategoriesController(CMSShoppingCartContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// GET /admin/categories
        /// </summary>
        public async Task<IActionResult> Index()
        {
            return View(await context.Categories.OrderBy(x => x.Sorting).ToListAsync());
        }

        /// <summary>
        /// GET /admin/categories/create
        /// </summary>
        public IActionResult Create() 
        {
            return View();
        } 


        /// <summary>
        /// POST /admin/categories/create
        /// </summary>
        /// <param name="category">Form data used to create the record</param>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                category.Sorting = 100;

                var slug = await context.Categories.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "A Category with this name already exists.");
                    return View(category);
                }

                context.Add(category);
                await context.SaveChangesAsync();

                TempData["Success"] = $"New category '{category.Name}' has been added.";

                return RedirectToAction("Index");
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                ModelState.AddModelError("", messages);
            }

            return View(category);
        }

        /// <summary>
        /// GET /admin/categories/edit/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to edit</param>
        public async Task<IActionResult> Edit(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)  
            {
                return NotFound();
            }
            return View(category);
        }

        /// <summary>
        /// POST /admin/categories/edit/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to edit</param>
        /// <param name="category">Form data used to edit the record</param>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");

                var slug = await context.Categories
                                        .Where(x => x.Id != id)
                                        .FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "A Category with this name already exists.");
                    return View(category);
                }

                context.Update(category);
                await context.SaveChangesAsync();

                TempData["Success"] = $"Category '{category.Name}' has been updated.";

                // NOTE: Implied key names like JS 🤔
                return RedirectToAction("Edit", new { id });
            }
            else

            return View(category);
        }

        /// <summary>
        /// GET /admin/categories/delete/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to delete</param>
        public async Task<IActionResult> Delete(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)  
            {
                TempData["Error"] = "You can not delete that which does not exist <br/> - the developer";
            }
            else 
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                TempData["Success"] = $"Category '{category.Name}' has been deleted";
            }
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// POST /admin/categories/reorder
        /// </summary>
        /// <param name="id">Form data containing new order data</param>
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var categoryId in id)
            {
                var category = await context.Categories.FindAsync(categoryId);
                category.Sorting = count;
                context.Update(category);
                await context.SaveChangesAsync();
                count++;
            }

            return Ok();
        }

    }
}
