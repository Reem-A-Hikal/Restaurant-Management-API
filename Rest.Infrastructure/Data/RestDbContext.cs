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
        public DbSet<DeliveryPerson> DeliveryPeople { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Review> Reviews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Chef>().ToTable("Chefs");
            modelBuilder.Entity<DeliveryPerson>().ToTable("DeliveryPersons");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            // Category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("IX_Categories_Name");

            modelBuilder.Entity<Category>()
                .Property(c => c.Status)
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            // Product
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.Name, p.CategoryId })
                .IsUnique()
                .HasDatabaseName("IX_Products_Name_CategoryId");

            modelBuilder.Entity<Product>()
                .Property(p => p.Status)
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            // Check Price > 0
            modelBuilder.Entity<Product>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Products_Price",
                    "[Price] > 0"));

            // Check DiscountPercent <= AllowedDiscountPercent
            modelBuilder.Entity<Product>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Products_Discount",
                    "[DiscountPercent] <= [AllowedDiscountPercent]"));

            // Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.CustomerOrders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryPerson)
                .WithMany(u => u.DeliveryOrders)
                .HasForeignKey(o => o.DeliveryPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryAddress)
                .WithMany()
                .HasForeignKey(o => o.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ConfirmedBy)
                .WithMany()
                .HasForeignKey(o => o.ConfirmedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // TotalAmount computed column
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasComputedColumnSql(
                    "[SubTotal] + [DeliveryFee] + [Tax] - [Discount]",
                    stored: true);

            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentStatus)
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasDatabaseName("IX_Orders_OrderNumber");

            modelBuilder.Entity<Order>()
    .           HasIndex(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.DeliveryPersonId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);

            // Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Method)
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.OrderId)
                .HasDatabaseName("IX_Payments_OrderId");

            // Order Detail
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Subtotal)
                .HasComputedColumnSql("[Quantity] * [UnitPrice]", stored: true);

            // Review
            modelBuilder.Entity<Review>()
                .Property(r => r.ReviewDate)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
