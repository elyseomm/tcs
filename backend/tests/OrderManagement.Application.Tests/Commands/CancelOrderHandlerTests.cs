using Moq;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Features.Orders.Commands;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using Xunit;
namespace OrderManagement.Application.Tests.Commands;
public sealed class CancelOrderHandlerTests
{
    [Fact]public async Task Handle_Cancels_pending_order()
    {
        var order = new Order(Guid.NewGuid(),[("Product", 1, 10m)]);
        var repo = new Mock<IOrderRepository>();
        repo.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
        var h = new CancelOrderHandler(repo.Object);
        await h.Handle( new(order.Id), default );
        Assert.Equal( OrderStatus.Cancelled,order.Status );
        repo.Verify(x=>x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]public async Task Handle_Throws_when_order_not_found()
    {
        var repo = new Mock<IOrderRepository>();
        var h = new CancelOrderHandler(repo.Object);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => h.Handle(new(Guid.NewGuid()), default));
    }
}
