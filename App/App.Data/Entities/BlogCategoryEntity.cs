using App.Data.Infrastructure;
using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

public class BlogCategoryEntity : EntityBase
{
    public string Name { get; set; } = null!;

    // Navigation properties
    public ICollection<RelBlogCategoryEntity> BlogRelations { get; set; } = [];
}

internal class BlogCategoryEntityConfiguration : IEntityTypeConfiguration<BlogCategoryEntity>
{
    public void Configure(EntityTypeBuilder<BlogCategoryEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
        builder.Property(e => e.CreatedAt).IsRequired();

        new BlogCategoryEntitySeed().SeedData(builder);
    }
}

internal class BlogCategoryEntitySeed : IEntityTypeSeed<BlogCategoryEntity>
{
    public void SeedData(EntityTypeBuilder<BlogCategoryEntity> builder)
    {
        builder.HasData(DemoData.BlogCategories);
    }
}
