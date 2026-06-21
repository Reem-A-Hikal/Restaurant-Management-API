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
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.ToTable("Deliveries");

            builder.HasKey(d => d.DeliveryId);

            builder.Property(d => d.StatusChangeTime)
                   .IsRequired();

            builder.Property(d => d.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(d => d.DeliveryStartTime);
            builder.Property(d => d.DeliveryEndTime);

            builder.Property(d => d.Notes)
                   .HasMaxLength(500);

            builder.Property(d => d.Latitude)
                   .HasColumnType("decimal(9,6)");

            builder.Property(d => d.Longitude)
                   .HasColumnType("decimal(9,6)");

            builder.Property(d => d.OrderId)
                   .IsRequired();

            builder.Property(d => d.DeliveryPersonId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.HasIndex(d => d.DeliveryPersonId)
                   .HasDatabaseName("IX_Deliveries_DeliveryPersonId");

            builder.HasIndex(d => d.OrderId)
                   .HasDatabaseName("IX_Deliveries_OrderId");

            builder.HasOne(d => d.DeliveryPerson)
                   .WithMany(u => u.Deliveries)
                   .HasForeignKey(d => d.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(d => d.Order)
                   .WithMany(o => o.Deliveries)
                   .HasForeignKey(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasQueryFilter(d =>
                d.Order.User.Status != UserStatus.Deleted);
        }
    }
}
