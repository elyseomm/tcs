using Moq;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Features.Orders.Queries;
using OrderManagement.Domain.Entities;
using Xunit;
namespace OrderManagement.Application.Tests.Queries;
public sealed class GetOrderByIdHandlerTests
{
    [Fact]public async Task Handle_Returns_order()
    {
        var order = new Order(Guid.NewGuid(), [("A", 1, 10m)]);
        var repo = new Mock<IOrderRepository>();
        repo.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var result = await new GetOrderByIdHandler(repo.Object).Handle(new(order.Id), default);
        Assert.Equal(order.Id, result.Id);
    }
}