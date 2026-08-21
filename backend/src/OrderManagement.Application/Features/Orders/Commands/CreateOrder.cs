using FluentValidation;
using MediatR; 
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Models;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Features.Orders.Commands;
public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyCollection<CreateOrderItem> Items):IRequest<OrderDto>;
public sealed record CreateOrderItem(string ProductName,int Quantity,decimal UnitPrice);
public sealed class CreateOrderValidator:AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x=>x.CustomerId).NotEmpty();
        RuleFor(x=>x.Items).NotEmpty();
        RuleForEach(x=>x.Items).SetValidator(new ItemValidator());
    }
    private sealed class ItemValidator:AbstractValidator<CreateOrderItem>
    {
        public ItemValidator()
        {
            RuleFor(x=>x.ProductName).NotEmpty();
            RuleFor(x=>x.Quantity).GreaterThan(0);
            RuleFor(x=>x.UnitPrice).GreaterThan(0);
        }
    }
}
public sealed class CreateOrderHandler(IOrderRepository repository):IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request,CancellationToken ct)
    {
        var order = new Order(request.CustomerId,request.Items.Select(x => (x.ProductName, x.Quantity, x.UnitPrice)));
        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);
        return Mapper.ToDto(order);
    }
}
internal static class Mapper { 
    public static OrderDto ToDto(Order x) => new(
        x.Id,
        x.CustomerId,
        x.Status,
        x.CreatedAt,
        x.TotalAmount,
        [.. x.Items.Select(i => new OrderItemDto(i.Id, i.ProductName, i.Quantity, i.UnitPrice))]
    ); 
}
