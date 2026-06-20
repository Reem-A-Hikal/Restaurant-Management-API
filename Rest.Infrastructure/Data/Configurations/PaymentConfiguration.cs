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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.PaymentId);

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Method)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar(20)")
                   .IsRequired();

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar(20)")
                   .IsRequired();

            builder.Property(p => p.TransactionId)
                   .HasMaxLength(100);

            builder.Property(p => p.GatewayResponse)
                   .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(p => p.OrderId)
                   .HasDatabaseName("IX_Payments_OrderId");

            builder.HasOne(p => p.Order)
                   .WithMany(o => o.Payments)
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(p => p.Order.User.Status != UserStatus.Deleted);
        }
    }
}
