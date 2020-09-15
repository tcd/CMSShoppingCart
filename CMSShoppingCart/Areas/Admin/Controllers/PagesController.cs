using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CMSShoppingCart.Infrastructure;
using CMSShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in context.Pages orderby p.Sorting select p;

            List<Page> pagesList = await pages.ToListAsync();

            return View(pagesList);
        }
    }
}
