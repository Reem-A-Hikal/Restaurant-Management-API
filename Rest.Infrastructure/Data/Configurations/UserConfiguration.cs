using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;

namespace Rest.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasQueryFilter(u => u.Status != UserStatus.Deleted);

            builder.Property(u => u.FullName)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("nvarchar(100)");

            builder.Property(u => u.ProfileImageUrl)
                   .HasMaxLength(255)
                   .HasColumnType("nvarchar(255)");

            builder.Property(u => u.Status)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar(20)")
                   .IsRequired();

            builder.HasIndex(u => u.Email)
                   .IsUnique()
                   .HasFilter("[Email] IS NOT NULL");

            builder.HasIndex(u => u.UserName)
                   .IsUnique()
                   .HasFilter("[UserName] IS NOT NULL");
        }
    }
}
