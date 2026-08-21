using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Abstractions;
using OrderManagement.Domain.Entities;
namespace OrderManagement.Infrastructure.Persistence;
public sealed class OrderRepository(AppDbContext db): IOrderRepository
{
    public Task AddAsync(Order o, CancellationToken ct) => db.Orders.AddAsync(o,ct).AsTask();
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) => db.Orders.Include(x => x.Items)
        .SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<(IReadOnlyCollection<Order> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var q = db.Orders.Include(x => x.Items).OrderByDescending(x => x.CreatedAt);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return(items, total);
    }
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
