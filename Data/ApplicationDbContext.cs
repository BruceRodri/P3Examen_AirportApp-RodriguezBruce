using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using P3Examen_AirportApp.Models.Commerce;

namespace P3Examen_AirportApp.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AirportService> AirportServices => Set<AirportService>();
    public DbSet<ShoppingCartItem> ShoppingCartItems => Set<ShoppingCartItem>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<ServiceAvailability> ServiceAvailabilities => Set<ServiceAvailability>();
    public DbSet<ServiceReservation> ServiceReservations => Set<ServiceReservation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("airportdb");

        builder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasIndex(p => p.PayPalOrderId).IsUnique();
        });

        builder.Entity<ServiceAvailability>(entity =>
        {
            entity.HasIndex(a => new { a.AirportServiceId, a.AirportId, a.ServiceDate, a.StartTime }).IsUnique();
            entity.HasOne(a => a.AirportService)
                .WithMany()
                .HasForeignKey(a => a.AirportServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ServiceReservation>(entity =>
        {
            entity.HasOne(r => r.AirportService)
                .WithMany()
                .HasForeignKey(r => r.AirportServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(r => r.PurchaseOrder)
                .WithMany(o => o.Reservations)
                .HasForeignKey(r => r.PurchaseOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
