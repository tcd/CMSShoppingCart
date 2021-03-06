﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMSShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace CMSShoppingCart.Infrastructure
{
    public class CMSShoppingCartContext : DbContext
    {
        public CMSShoppingCartContext(DbContextOptions<CMSShoppingCartContext> options)
            :base(options)
        {
        }

        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
