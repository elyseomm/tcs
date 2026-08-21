using OrderManagement.Domain.Entities;
namespace OrderManagement.Application.Abstractions;
public interface IOrderRepository
{
 Task AddAsync(Order order, CancellationToken cancellationToken);
 Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
 Task<(IReadOnlyCollection<Order> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
 Task SaveChangesAsync(CancellationToken cancellationToken);
}
