using App.Data.Infrastructure;
using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

public class ProductImageEntity : EntityBase
{
    public int ProductId { get; set; }
    public string Url { get; set; } = null!;

    /// <summary>
    /// File API üzerinde dosyayı tekil olarak tanımlayan anahtar. Görsel File API üzerinden
    /// silinirken veya public URL yeniden üretilirken kullanılır. Seed verisi gibi File API'den
    /// bağımsız statik görseller için null olabilir.
    /// </summary>
    public string? FileKey { get; set; }

    // Navigation properties
    public ProductEntity Product { get; set; } = null!;
}

internal class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImageEntity>
{
    public void Configure(EntityTypeBuilder<ProductImageEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ProductId).IsRequired();
        builder.Property(e => e.Url).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasOne(d => d.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        new ProductImageEntitySeed().SeedData(builder);
    }
}

internal class ProductImageEntitySeed : IEntityTypeSeed<ProductImageEntity>
{
    public void SeedData(EntityTypeBuilder<ProductImageEntity> builder)
    {
        builder.HasData(DemoData.ProductImages);
    }
}