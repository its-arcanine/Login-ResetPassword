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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entities here
            // Example: modelBuilder.Entity<Account>().ToTable("Accounts");
            base.OnModelCreating(modelBuilder);
        }

        // Define DbSets for your entities
        // public DbSet<Account> Accounts { get; set; }
        // Add other DbSets as needed
    }
}
