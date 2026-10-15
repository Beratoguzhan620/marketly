using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

public enum OrderStatus : byte
{
    Preparing = 0,
    Shipped = 1,
    Delivered = 2,
    Cancelled = 3,
}

public class OrderEntity : EntityBase
{
    public int UserId { get; set; }
    public string OrderCode { get; set; } = null!;
    public string Address { get; set; } = null!;
    public OrderStatus Status { get; set; } = OrderStatus.Preparing;
    public DateTime? StatusUpdatedAt { get; set; }
    public string? CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }

    // Navigation properties
    public UserEntity User { get; set; } = null!;

    public ICollection<OrderItemEntity> OrderItems { get; set; } = [];
}

internal class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.OrderCode).IsRequired().HasMaxLength(250);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(250);
        builder.Property(e => e.Status).IsRequired().HasDefaultValue(OrderStatus.Preparing);
        builder.Property(e => e.CouponCode).HasMaxLength(32);
        builder.Property(e => e.DiscountAmount).HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => e.OrderCode).IsUnique();
        builder.HasIndex(e => e.Status);

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(DemoData.Orders);
    }
}
