using BookStore.Models;
using BookStore.Models.ApplicationUser;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(obj =>
            {
                obj.Property(x => x.ImageUrl).HasMaxLength(150);
                obj.Property(x => x.Price).HasColumnType("decimal(5, 2)");
                obj.Property(x => x.Price50).HasColumnType("decimal(5, 2)");
                obj.Property(x => x.Price100).HasColumnType("decimal(5, 2)");
                obj.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
                obj.HasOne(x => x.CoverType).WithMany().HasForeignKey(x => x.CoverTypeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AppUser>(obj =>
            {
                obj.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ShoppingCartItem>(obj =>
            {
                obj.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
                obj.HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.AppUserId);
            });

            modelBuilder.Entity<Order>(obj =>
            {
                obj.HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.AppUserId);
            });

            modelBuilder.Entity<OrderDetail>(obj =>
            {
                obj.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId);
                obj.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
            });
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
