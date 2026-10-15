using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

public class RelBlogTagEntity : EntityBase
{
    public int BlogId { get; set; }
    public int TagId { get; set; }

    // Navigation properties
    public BlogEntity Blog { get; set; } = null!;

    public BlogTagEntity Tag { get; set; } = null!;
}

internal class RelBlogTagEntityConfiguration : IEntityTypeConfiguration<RelBlogTagEntity>
{
    public void Configure(EntityTypeBuilder<RelBlogTagEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.BlogId).IsRequired();
        builder.Property(e => e.TagId).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.HasIndex(e => new { e.BlogId, e.TagId }).IsUnique();

        builder.HasOne(d => d.Blog)
            .WithMany(b => b.TagLinks)
            .HasForeignKey(d => d.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Tag)
            .WithMany(t => t.BlogRelations)
            .HasForeignKey(d => d.TagId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(DemoData.BlogTagLinks);
    }
}
