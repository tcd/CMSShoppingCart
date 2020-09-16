﻿using System;
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
        private readonly CmsShoppingCartContext context;

        public PagesController(CmsShoppingCartContext context)
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
            return View(page);
        }

        /// <summary>
        /// GET /admin/pages/create
        /// </summary>
        public IActionResult Create() => View();


        /// <summary>
        /// POST /admin/pages/create
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "A Page with this title already exists.");
                    return View(page);
                }

                context.Add(page);
                await context.SaveChangesAsync();

         
                return RedirectToAction("Index", "Pages");
            }

            return View(page);
        }
    }
}
