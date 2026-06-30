using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;

namespace Rest.Infrastructure.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.AddressId);

            builder.Property(a => a.AddressLine1)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.AddressLine2)
                .HasMaxLength(255);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Governorate)
                .HasMaxLength(100);

            builder.Property(a => a.Latitude)
                .HasColumnType("float");

            builder.Property(a => a.Longitude)
                .HasColumnType("float");

            builder.Property(a => a.AddressType)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            builder.Property(a => a.IsDefault)
                .IsRequired();

            builder.Property(a => a.UserId)
                .IsRequired();

            builder.HasOne(a => a.User)
                   .WithMany(u => u.Addresses)
                   .HasForeignKey(a => a.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.UserId);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Addresses_Latitude", "[Latitude] >= -90 AND [Latitude] <= 90 OR [Latitude] IS NULL");
                t.HasCheckConstraint("CK_Addresses_Longitude", "[Longitude] >= -180 AND [Longitude] <= 180 OR [Longitude] IS NULL");
            });

            builder.HasQueryFilter(a => a.User.Status != UserStatus.Deleted);
        }
    }
}
