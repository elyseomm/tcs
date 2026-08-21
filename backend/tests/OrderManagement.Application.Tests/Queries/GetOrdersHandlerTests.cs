using Moq;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Features.Orders.Queries;
using OrderManagement.Domain.Entities;
using Xunit;
namespace OrderManagement.Application.Tests.Queries;
public sealed class GetOrdersHandlerTests{[Fact]public async Task Handle_Returns_paged_orders()
{
    var orders = new[]
    {
        new Order(Guid.NewGuid(),[("A", 1, 10m)])};
        var repo = new Mock<IOrderRepository>();
        repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync((orders, 1));
        var result=await new GetOrdersHandler(repo.Object).Handle(new(1,10), default);
        Assert.Single(result.Items);
        Assert.Equal(1,result.TotalCount);
    }
}