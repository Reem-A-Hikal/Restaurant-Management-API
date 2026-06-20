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

            builder.Property(od => od.UnitPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(od => od.Subtotal)
                   .HasColumnType("decimal(18,2)");

            builder.Property(od => od.Subtotal)
                   .HasComputedColumnSql("[Quantity] * [UnitPrice]", stored: true);

            builder.Property(od => od.SpecialInstructions)
                   .HasMaxLength(500);

            builder.HasOne(od => od.Order)
                   .WithMany(o => o.OrderDetails)
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(od => od.Order.User.Status != UserStatus.Deleted);

            builder.HasOne(od => od.Product)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(od => od.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
