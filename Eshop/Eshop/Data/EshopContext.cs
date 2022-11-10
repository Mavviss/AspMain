using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eshop.Models;

namespace Eshop.Data
{
    public class EshopContext : DbContext
    {
        public EshopContext (DbContextOptions<EshopContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; } = default!;
        public DbSet<Cart> Carts { get; set; } = default!;
        public DbSet<Invoice> Invoices { get; set; } = default!;
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductType> ProductTypes { get; set; } = default!;
    }
}
