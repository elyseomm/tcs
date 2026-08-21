using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Features.Orders.Commands;
using OrderManagement.Application.Models;
namespace OrderManagement.Application.Features.Orders.Queries;
public sealed record GetOrderByIdQuery(Guid Id):IRequest<OrderDto>;
public sealed class GetOrderByIdHandler(IOrderRepository repository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await repository.GetByIdAsync(request.Id, ct) ??
            throw new KeyNotFoundException("Order not found."); 
        return Mapper.ToDto(order);
    }
}

