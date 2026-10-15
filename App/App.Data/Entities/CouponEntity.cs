using App.Data.Infrastructure;
using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

/// <summary>
/// A cart-wide code entered at checkout. Not to be confused with <see cref="DiscountEntity"/>,
/// which is a percentage attached to individual products.
/// </summary>
public class CouponEntity : EntityBase, IHasEnabled
{
    public string Code { get; set; } = null!;
    public byte DiscountRate { get; set; }
    public decimal? MinimumOrderTotal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public bool Enabled { get; set; } = true;
}

internal class CouponEntityConfiguration : IEntityTypeConfiguration<CouponEntity>
{
    public void Configure(EntityTypeBuilder<CouponEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).IsRequired().HasMaxLength(32);
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.MinimumOrderTotal).HasPrecision(18, 2);
        builder.Property(e => e.UsedCount).IsRequired().HasDefaultValue(0);
        builder.Property(e => e.Enabled).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.ToTable(t => t.HasCheckConstraint("CK_Coupons_DiscountRate", "[DiscountRate] BETWEEN 1 AND 90"));

        builder.HasData(DemoData.Coupons);
    }
}
