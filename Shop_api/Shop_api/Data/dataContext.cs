using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop_api.Models;

namespace Shop_api.Data
{
    public class dataContext : IdentityDbContext<User>
    {
        public dataContext(DbContextOptions<dataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        //public dataContext()
        //{
        //    Database.EnsureCreated();
        //}

        public DbSet<Group> Groups { get; set; }
        public DbSet<Product> Products { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<Product>()
        //        .HasOne(p => p.Group)
        //        .WithMany(g => g.Products)
        //        .HasForeignKey(p => p.GroupId);

        //    //modelBuilder.Entity<Group>()
        //    //   .HasMany(g => g.Products)
        //    //   .WithOne(p => p.Group)
        //    //   .HasForeignKey(g => g.GroupId);
        //}
    }
}
