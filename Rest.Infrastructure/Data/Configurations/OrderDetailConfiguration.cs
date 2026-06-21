using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Infrastructure.Data.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");
            builder.HasKey(od => od.OrderDetailId);

            builder.Property(od => od.OrderId)
                   .IsRequired();

            builder.Property(od => od.ProductId)
                   .IsRequired();

            builder.Property(od => od.Quantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(od => od.UnitPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(od => od.Subtotal)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)")
                   .HasComputedColumnSql("[Quantity] * [UnitPrice]", stored: true);

            builder.Property(od => od.SpecialInstructions)
                   .HasMaxLength(500);

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_OrderDetails_Quantity",
                "[Quantity] >= 1"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_OrderDetails_UnitPrice",
                "[UnitPrice] > 0"));

            builder.HasIndex(od => od.OrderId)
                   .HasDatabaseName("IX_OrderDetails_OrderId");

            builder.HasIndex(od => od.ProductId)
                   .HasDatabaseName("IX_OrderDetails_ProductId");

            builder.HasOne(od => od.Order)
                   .WithMany(o => o.OrderDetails)
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasOne(od => od.Product)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(od => od.ProductId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasQueryFilter(od => od.Order.User.Status != UserStatus.Deleted);
        }
    }
}
