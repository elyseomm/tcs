using OrderManagement.Domain.Enums;
namespace OrderManagement.Application.Models;
public sealed record OrderItemDto(Guid Id, string ProductName, int Quantity, decimal UnitPrice);
public sealed record OrderDto(Guid Id, Guid CustomerId, OrderStatus Status, DateTime CreatedAt, decimal TotalAmount, IReadOnlyCollection<OrderItemDto> Items);
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount);
