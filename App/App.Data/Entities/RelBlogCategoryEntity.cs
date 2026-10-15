using App.Data.Infrastructure;
using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

public class RelBlogCategoryEntity : EntityBase
{
    public int BlogId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public BlogEntity Blog { get; set; } = null!;

    public BlogCategoryEntity Category { get; set; } = null!;
}

internal class RelBlogCategoryEntityConfiguration : IEntityTypeConfiguration<RelBlogCategoryEntity>
{
    public void Configure(EntityTypeBuilder<RelBlogCategoryEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.BlogId).IsRequired();
        builder.Property(e => e.CategoryId).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.HasIndex(e => new { e.BlogId, e.CategoryId }).IsUnique(); // Each blog and category pair should be unique

        builder.HasOne(d => d.Blog)
            .WithMany(b => b.CategoryLinks)
            .HasForeignKey(d => d.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Category)
            .WithMany(bc => bc.BlogRelations)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        new RelBlogCategoryEntitySeed().SeedData(builder);
    }
}

internal class RelBlogCategoryEntitySeed : IEntityTypeSeed<RelBlogCategoryEntity>
{
    public void SeedData(EntityTypeBuilder<RelBlogCategoryEntity> builder)
    {
        builder.HasData(DemoData.BlogCategoryLinks);
    }
}
