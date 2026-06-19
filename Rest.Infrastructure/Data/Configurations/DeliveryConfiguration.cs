using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rest.Domain.Entities;
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

            builder.Property(d => d.Status)
                   .HasConversion<string>()
                   .HasColumnType("varchar(20)")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(d => d.Notes)
                   .HasMaxLength(500);

            builder.Property(d => d.Latitude)
                   .HasColumnType("decimal(9,6)");

            builder.Property(d => d.Longitude)
                   .HasColumnType("decimal(9,6)");

            builder.HasIndex(d => d.OrderId)
                   .IsUnique();

            builder.HasIndex(d => d.DeliveryPersonId);

            builder.HasOne(d => d.DeliveryPerson)
                   .WithMany(u => u.Deliveries)
                   .HasForeignKey(d => d.DeliveryPersonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Order)
                   .WithOne(o => o.Delivery)
                   .HasForeignKey<Delivery>(d => d.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
