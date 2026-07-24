using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Data
{
    public class RestDbContext : IdentityDbContext<User>
    {
        public RestDbContext(DbContextOptions<RestDbContext> options) : base(options)
        {}

        public DbSet<Chef> Chefs { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestDbContext).Assembly);
        }
    }
}
