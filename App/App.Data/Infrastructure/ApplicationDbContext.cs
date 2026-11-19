namespace App.Data.Infrastructure;

using App.Data.Entities;
using Microsoft.EntityFrameworkCore;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductImageEntity> ProductImages { get; set; }
    public DbSet<ProductCommentEntity> ProductComments { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<CartItemEntity> CartItems { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }
    public DbSet<ContactFormEntity> ContactForms { get; set; }
    public DbSet<DiscountEntity> ProductDiscounts { get; set; }
    public DbSet<CouponEntity> Coupons { get; set; }
    public DbSet<BlogEntity> Blogs { get; set; }
    public DbSet<BlogCommentEntity> BlogComments { get; set; }
    public DbSet<BlogCategoryEntity> BlogCategories { get; set; }
    public DbSet<BlogTagEntity> BlogTags { get; set; }
    public DbSet<RelBlogTagEntity> RelBlogTags { get; set; }
    public DbSet<RelBlogCategoryEntity> RelBlogCategories { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQLite (the public demo) has no decimal type: EF stores decimals as text there, which cannot be ordered,
        // summed or range-checked. As REAL they can; prices, totals and rates stay well within its precision. SQL Server keeps decimal.
        if (Database.IsSqlite())
            configurationBuilder.Properties<decimal>().HaveConversion<double>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCommentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContactFormEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CouponEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BlogEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BlogCommentEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BlogCategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new BlogTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RelBlogTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RelBlogCategoryEntityConfiguration());
    }
}