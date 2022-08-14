﻿namespace BookStore.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
#pragma warning disable CS8618 // Non-nullable property 'Orders' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'CoverTypes' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'Products' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'ShippingCompanies' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'Categories' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'OrderDetails' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'Companies' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'ShoppingCartItems' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
#pragma warning restore CS8618 // Non-nullable property 'ShoppingCartItems' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'Companies' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'OrderDetails' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'Categories' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'ShippingCompanies' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'Products' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'CoverTypes' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'Orders' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(x => x.ImageUrl).HasMaxLength(150);
                entity.Property(x => x.Price).HasColumnType("decimal(5, 2)");
                entity.Property(x => x.Price50).HasColumnType("decimal(5, 2)");
                entity.Property(x => x.Price100).HasColumnType("decimal(5, 2)");
                entity.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.CoverType).WithMany().HasForeignKey(x => x.CoverTypeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
                entity.HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.AppUserId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(x => x.AppUser).WithMany().HasForeignKey(x => x.AppUserId);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId);
                entity.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
            });
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CoverType> CoverTypes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<ShippingCompany> ShippingCompanies { get; set; }
    }
}
