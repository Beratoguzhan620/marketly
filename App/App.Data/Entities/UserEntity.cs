using App.Data.Infrastructure;
using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

public class UserEntity : EntityBase, IHasEnabled
{
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? ResetPasswordToken { get; set; }
    public int RoleId { get; set; }
    public bool Enabled { get; set; } = true;
    public bool HasSellerRequest { get; set; } = false;

    // Navigation properties
    public RoleEntity Role { get; set; } = null!;
}

internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Password).IsRequired();
        builder.Property(e => e.RoleId).IsRequired();
        builder.Property(e => e.Enabled).IsRequired().HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasOne(d => d.Role)
            .WithMany()
            .HasForeignKey(d => d.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        new UserEntitySeed().SeedData(builder);
    }
}

internal class UserEntitySeed : IEntityTypeSeed<UserEntity>
{
    public void SeedData(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasData(DemoData.Users);
    }
}