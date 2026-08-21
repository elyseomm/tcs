using OrderManagement.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
namespace OrderManagement.Domain.Entities;
public sealed class OrderItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }

    [DataType(DataType.Text), MaxLength(250)]
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    private OrderItem() { }
    public OrderItem(Guid orderId, string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName)) throw new DomainException("Nome do Produto é requerido.");
        if (quantity <= 0) throw new DomainException("Quantidade precisa ser maior que zero.");
        if (unitPrice <= 0) throw new DomainException("UnitPrice precisa ser maior que zero.");
        OrderId = orderId; ProductName = productName.Trim(); Quantity = quantity; UnitPrice = unitPrice;
    }
    public decimal Total => Quantity * UnitPrice;
}
