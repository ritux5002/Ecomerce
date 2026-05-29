using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiEcommerce.Domain.Entities;

namespace MiEcommerce.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name);

        // Seed con 3 categorías
        var electronicaId = new Guid("11111111-1111-1111-1111-111111111111");
        var ropaId = new Guid("22222222-2222-2222-2222-222222222222");
        var hogarId = new Guid("33333333-3333-3333-3333-333333333333");

        builder.HasData(
            Category.CreateWithId(electronicaId, "Electrónica"),
            Category.CreateWithId(ropaId, "Ropa"),
            Category.CreateWithId(hogarId, "Hogar")
        );
    }
}
