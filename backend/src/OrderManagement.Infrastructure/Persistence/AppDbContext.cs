using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
namespace OrderManagement.Infrastructure.Persistence;
public sealed class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
{
    public DbSet<Order> Orders=>Set<Order>();
    public DbSet<OrderItem> OrderItems=>Set<OrderItem>();
    protected override void OnModelCreating(ModelBuilder b){
        b.Entity<Order>(e=> {
            e.HasKey(x=>x.Id);
            e.Property(x=>x.Status).HasConversion<string>();
            e.HasMany(x=>x.Items).WithOne().HasForeignKey(x=>x.OrderId).OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<OrderItem>(e=>{
            e.HasKey(x=>x.Id);
            e.Property(x=>x.ProductName).HasMaxLength(250);
            e.Property(x=>x.UnitPrice).HasPrecision(18,2);
        });
    }
}
