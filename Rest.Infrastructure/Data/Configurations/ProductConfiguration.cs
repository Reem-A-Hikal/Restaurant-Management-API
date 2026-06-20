using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;

namespace Rest.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasQueryFilter(p => p.Status != ProductStatus.Archived
                          && p.Category!.Status != CategoryStatus.Archived);

            builder.ToTable("Products");
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Products_Price", "[Price] > 0");
                t.HasCheckConstraint("CK_Products_Discount", "[DiscountPercent] <= [AllowedDiscountPercent]");
            });

            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("nvarchar(100)");

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Description)
                   .HasMaxLength(1000)
                   .HasColumnType("nvarchar(1000)");

            builder.Property(p => p.ImageUrl)
                   .HasMaxLength(255)
                   .HasColumnType("nvarchar(255)");

            builder.Property(p => p.DiscountPercent)
                   .HasColumnType("decimal(5,2)");

            builder.Property(p => p.AllowedDiscountPercent)
                   .HasColumnType("decimal(5,2)");

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar(20)")
                   .IsRequired();

            builder.HasIndex(p => new { p.Name, p.CategoryId })
                   .IsUnique()
                   .HasDatabaseName("IX_Products_Name_CategoryId");

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
