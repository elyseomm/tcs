using FluentValidation;
using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Features.Orders.Commands;
using OrderManagement.Application.Models;

namespace OrderManagement.Application.Features.Orders.Queries;
public sealed record GetOrdersQuery(int Page=1, int PageSize=10): IRequest<PagedResult<OrderDto>>;
public sealed class GetOrdersValidator:AbstractValidator<GetOrdersQuery>
{
    public GetOrdersValidator()
    { 
        RuleFor(x=>x.Page).GreaterThan(0); 
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
public sealed class GetOrdersHandler(IOrderRepository repository): IRequestHandler<GetOrdersQuery,PagedResult<OrderDto>>
{ 
    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request,CancellationToken ct)
    {
        var r = await repository.GetPagedAsync(request.Page, request.PageSize, ct);
        return new(r.Items.Select(Mapper.ToDto).ToList(), request.Page, request.PageSize, r.TotalCount);
    }
}
