using MediatR;
using OrderManagement.Application.Abstractions;
namespace OrderManagement.Application.Features.Orders.Commands;
public sealed record CancelOrderCommand(Guid OrderId):IRequest;
public sealed class CancelOrderHandler(IOrderRepository repository): IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await repository.GetByIdAsync(request.OrderId,ct) ??
            throw new KeyNotFoundException("Order not found.");
        order.Cancel();
        await repository.SaveChangesAsync(ct);
    }
}
