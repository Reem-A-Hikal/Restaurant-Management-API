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
                    .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(o => o.RequiredTime);
            builder.Property(o => o.ConfirmationTime);
            builder.Property(o => o.PreparationStartTime);
            builder.Property(o => o.CancellationTime);

            builder.Property(o => o.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(o => o.ConfirmedById)
                  .HasMaxLength(450);

            builder.Property(o => o.SubTotal)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DeliveryFee)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Tax)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Discount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0.00m);

            builder.Property(o => o.TotalAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)")
                   .HasComputedColumnSql("[SubTotal] + [DeliveryFee] + [Tax] - [Discount]", stored: true);

            builder.Property(o => o.EstimatedDeliveryTime);

            builder.Ignore(o => o.PaymentStatus);
            builder.Ignore(o => o.IsPaid);

            builder.Property(o => o.StaffNotes)
                   .HasMaxLength(1000);

            builder.Property(o => o.CustomerNotes)
                   .HasMaxLength(1000);

            builder.Property(o => o.Source)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(o => o.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(o => o.DeliveryAddressId)
                   .IsRequired();

            builder.HasIndex(o => o.OrderNumber)
                   .IsUnique()
                   .HasDatabaseName("IX_Orders_OrderNumber");

            builder.HasIndex(o => o.Status)
                   .HasDatabaseName("IX_Orders_Status");

            builder.HasIndex(o => o.UserId)
                   .HasDatabaseName("IX_Orders_UserId");

            builder.HasOne(o => o.User)
                   .WithMany(u => u.CustomerOrders)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(o => o.DeliveryAddress)
                   .WithMany()
                   .HasForeignKey(o => o.DeliveryAddressId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(o => o.ConfirmedBy)
                   .WithMany()
                   .HasForeignKey(o => o.ConfirmedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderDetails)
                   .WithOne(od => od.Order)
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Payments)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Deliveries)
                   .WithOne(d => d.Order)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Review)
                   .WithOne(r => r.Order)
                   .HasForeignKey<Review>(r => r.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(a => a.User.Status != UserStatus.Deleted);
        }
    }
}
