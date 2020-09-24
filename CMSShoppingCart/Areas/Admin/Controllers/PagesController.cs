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
    public class PagesController : Controller
    {
        private readonly CMSShoppingCartContext context;

        public PagesController(CMSShoppingCartContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// GET /admin/pages
        /// </summary>
        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in context.Pages orderby p.Sorting select p;
            List<Page> pagesList = await pages.ToListAsync();
            return View(pagesList);
        }

        /// <summary>
        /// GET /admin/pages/details/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to show</param>
        public async Task<IActionResult> Details(int id)
        {
            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);
            if (page == null)  
            {
                return NotFound();
            }
            TempData["SlugPath"] = $"{Request.Scheme}://{Request.Host}/pages/{page.Slug}";
            return View(page);
        }

        /// <summary>
        /// GET /admin/pages/create
        /// </summary>
        public IActionResult Create() => View();


        /// <summary>
        /// POST /admin/pages/create
        /// </summary>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "A Page with this title already exists");
                    return View(page);
                }

                context.Add(page);
                await context.SaveChangesAsync();

                TempData["Success"] = $"New page '{page.Title}' has been added";

                return RedirectToAction("Index");
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                ModelState.AddModelError("", messages);
            }

            return View(page);
        }

        /// <summary>
        /// GET /admin/pages/edit/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to edit</param>
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await context.Pages.FindAsync(id);
            if (page == null)  
            {
                return NotFound();
            }
            return View(page);
        }

        /// <summary>
        /// POST /admin/pages/edit/{id}
        /// </summary>
        /// <param name="page">Form data used to edit the record</param>
        [
            HttpPost,
            ValidateAntiForgeryToken,
        ]
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = (page.Id == 1) ? "home" : page.Title.ToLower().Replace(" ", "-");

                var slug = await context.Pages
                                        .Where(x => x.Id != page.Id)
                                        .FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "A Page with this title already exists");
                    return View(page);
                }

                context.Update(page);
                await context.SaveChangesAsync();

                TempData["Success"] = $"Page '{page.Title}' has been updated";

                return RedirectToAction("Edit", new { id = page.Id });
            }
            else
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                ModelState.AddModelError("", messages);
            }

            return View(page);
        }

        /// <summary>
        /// GET /admin/pages/delete/{id}
        /// </summary>
        /// <param name="id">Primary key of the record to delete</param>
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await context.Pages.FindAsync(id);
            if (page == null)  
            {
                TempData["Error"] = "You can not delete that which does not exist <br/> - the developer";
            }
            else 
            {
                context.Pages.Remove(page);
                await context.SaveChangesAsync();

                TempData["Success"] = $"Page '{page.Title}' has been deleted";
            }
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// POST /admin/pages/reorder
        /// FIXME: change `id` param to `ids`.
        /// </summary>
        /// <param name="id">Form data containing new order data</param>
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var pageId in id)
            {
                Page page = await context.Pages.FindAsync(pageId);
                page.Sorting = count;
                context.Update(page);
                await context.SaveChangesAsync();
                count++;
            }

            return Ok();
        }

    }
}
