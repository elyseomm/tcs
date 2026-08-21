using Moq;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Features.Orders.Commands;
using Xunit;
namespace OrderManagement.Application.Tests.Commands;
public sealed class CreateOrderHandlerTests
{
    [Fact]public async Task Handle_Creates_order_and_calculates_total_in_domain()
    {
        var repo = new Mock<IOrderRepository>();
        var handler=new CreateOrderHandler(repo.Object);
        var result = await handler.Handle(
            new(Guid.NewGuid(),
            [
                new("Keyboard", 2, 100m),
                new("Mouse", 1, 50m)
            ]), default);

        Assert.Equal( 250m, result.TotalAmount);
        repo.Verify( x => x.AddAsync(It.IsAny<Domain.Entities.Order>(), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
        repo.Verify( x => x.SaveChangesAsync( It.IsAny<CancellationToken>()), Times.Once);
    }
}