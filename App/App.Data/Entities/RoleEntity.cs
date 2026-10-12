using App.Data.Infrastructure;
using App.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Data.Entities;

public class RoleEntity : EntityBase
{
    public string Name { get; set; } = null!;
}

internal class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(10);
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => e.Name).IsUnique();

        new RoleEntitySeed().SeedData(builder);
    }
}

internal class RoleEntitySeed : IEntityTypeSeed<RoleEntity>
{
    public void SeedData(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.HasData(DemoData.Roles);
    }
}