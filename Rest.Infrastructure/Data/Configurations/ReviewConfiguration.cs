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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.ReviewId);

            builder.Property(r => r.ReviewerName)
                .HasMaxLength(50);

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(r => r.ReviewDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.CustomerId)
                .IsRequired();

            builder.Property(r => r.OrderId)
                .IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] >= 1 AND [Rating] <= 5");
                t.HasCheckConstraint("CK_Reviews_DeliveryRating", "[DeliveryRating] >= 1 AND [DeliveryRating] <= 5 OR [DeliveryRating] IS NULL");
                t.HasCheckConstraint("CK_Reviews_FoodRating", "[FoodRating] >= 1 AND [FoodRating] <= 5 OR [FoodRating] IS NULL");
            });

            builder.HasOne(r => r.Customer)
                   .WithMany(u => u.Reviews)
                   .HasForeignKey(r => r.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Order)
                   .WithOne(o => o.Review)
                   .HasForeignKey<Review>(r => r.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Product)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(r => r.OrderId)
                .IsUnique();

            builder.HasIndex(r => r.CustomerId);
            builder.HasIndex(r => r.ProductId);

            builder.HasQueryFilter(r => r.Customer.Status != UserStatus.Deleted);
        }
    }
}
