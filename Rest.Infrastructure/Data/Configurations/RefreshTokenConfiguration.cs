using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration :IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.TokenHash)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(r => r.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(r => r.ReplacedByTokenHash)
                   .HasMaxLength(200);

            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.ExpiresAt).IsRequired();

            builder.HasIndex(r => r.TokenHash).IsUnique();
            builder.HasIndex(r => r.UserId);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(r => r.IsExpired);
            builder.Ignore(r => r.IsRevoked);
            builder.Ignore(r => r.IsActive);

            builder.HasQueryFilter(r => r.User.Status != UserStatus.Deleted);
        }
    }
}
