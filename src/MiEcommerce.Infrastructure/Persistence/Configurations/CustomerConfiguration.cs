using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiEcommerce.Domain.Entities;
using MiEcommerce.Domain.ValueObjects;

namespace MiEcommerce.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Email)
            .HasConversion(
                e => e.Value,
                e => new Email(e)
            )
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
