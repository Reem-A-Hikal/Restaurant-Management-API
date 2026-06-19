using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Data.Configurations
{
    public class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPerson>
    {
        public void Configure(EntityTypeBuilder<DeliveryPerson> builder)
        {
            builder.ToTable("DeliveryPersons");

            builder.Property(d => d.VehicleNumber)
                   .IsRequired()
                   .HasMaxLength(20);
        }
    }
}
