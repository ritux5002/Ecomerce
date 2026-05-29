using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiEcommerce.Domain.Entities;

namespace MiEcommerce.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Email).IsUnique();

        // Seed con usuario Admin
        var adminId = new Guid("99999999-9999-9999-9999-999999999999");
        // Hash de "admin123" pre-computado con BCrypt cost 11 — fijo para que HasData sea determinístico
        const string passwordHash = "$2a$11$5tBfpYDcfZL/IuJPitCuP.PuvHD0cwMv.h57BGZNnnj3V4JxPHnHu";

        builder.HasData(
            User.CreateWithId(adminId, "Admin", "admin@ecommerce.com", passwordHash, "Admin")
        );
    }
}
