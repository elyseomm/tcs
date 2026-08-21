using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace OrderManagement.Domain.Entities;
public sealed class Order
{
    private readonly List<OrderItem> _items = [];
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CustomerId { get; private set; }

    [DataType(DataType.Text), MaxLength(10)]
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(x => x.Total);
    private Order() { }
    public Order(Guid customerId, IEnumerable<(string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        CustomerId = customerId;
        foreach (var item in items) _items.Add(new OrderItem(Id, item.ProductName, item.Quantity, item.UnitPrice));
        if (_items.Count == 0) throw new DomainException("An order must contain at least one item.");
    }
    public void Cancel()
    {
        if (Status != OrderStatus.Pending) throw new DomainException("Only pending orders can be cancelled.");
        Status = OrderStatus.Cancelled;
    }
}
