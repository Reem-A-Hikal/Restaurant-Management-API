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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.OrderId);

            builder.Property(o => o.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(o => o.OrderDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(o => o.SubTotal)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DeliveryFee)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Tax)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Discount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TotalAmount)
                    .HasColumnType("decimal(18,2)")
                   .HasComputedColumnSql("[SubTotal] + [DeliveryFee] + [Tax] - [Discount]", stored: true);

            builder.Property(o => o.Notes)
                   .HasMaxLength(1000);

            builder.HasIndex(o => o.OrderNumber)
                   .IsUnique()
                   .HasDatabaseName("IX_Orders_OrderNumber");

            builder.HasIndex(o => o.Status)
                   .HasDatabaseName("IX_Orders_Status");

            builder.Ignore(o => o.PaymentStatus);

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.DeliveryPersonId);

            builder.HasOne(o => o.User)
                   .WithMany(u => u.CustomerOrders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(a => a.User.Status != UserStatus.Deleted);

            builder.HasOne(o => o.DeliveryPerson)
                   .WithMany(u => u.DeliveryOrders)
                   .HasForeignKey(o => o.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.DeliveryAddress)
                   .WithMany()
                   .HasForeignKey(o => o.DeliveryAddressId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.ConfirmedBy)
                   .WithMany()
                   .HasForeignKey(o => o.ConfirmedById)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
