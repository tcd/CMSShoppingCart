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
        private readonly CmsShoppingCartContext context;

        public CategoriesController(CmsShoppingCartContext context)
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

    }
}
