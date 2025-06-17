using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DataDbContext : DbContext 
    {
      
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
        }
        DbSet<Account> Accounts { get; set; } 
        DbSet<Product> Products { get; set; }
        DbSet<Cart> Carts { get; set; }
        DbSet<CartItem> CartItem { get; set; }

        DbSet<Order> Orders { get; set; }

        DbSet<Feedback> Feedbacks { get; set; }
        DbSet<Categories> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entities here
            // Example: modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<Account>().HasKey(a => a.AccountId);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
            modelBuilder.Entity<Cart>().HasKey(c => c.CartId);
            modelBuilder.Entity<CartItem>().HasKey(ci => ci.CartItemId);
            modelBuilder.Entity<Categories>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<Feedback>().HasKey(f => f.FeedbackId);


            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems) // Cart has a collection of CartItems
                .WithOne(ci => ci.Cart) // Each CartItem belongs to one Cart
                .HasForeignKey(ci => ci.CartId) // The foreign key in CartItem is CartId
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);

        }

        // Define DbSets for your entities
        // public DbSet<Account> Accounts { get; set; }
        // Add other DbSets as needed
    }
}
