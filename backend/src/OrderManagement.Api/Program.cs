using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Api;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Behaviors;
using OrderManagement.Application.Features.Orders.Commands;
using OrderManagement.Application.Features.Orders.Queries;
using OrderManagement.Infrastructure;
using OrderManagement.Infrastructure.Persistence;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx,_,cfg) => 
    cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console()
);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR( c => c.RegisterServicesFromAssemblyContaining<CreateOrderCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommand>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>),typeof(LoggingBehavior<,>));

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new()
{
    ValidateIssuer = true,ValidIssuer=jwt["Issuer"],ValidateAudience=true,ValidAudience=jwt["Audience"],
    ValidateIssuerSigningKey=true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)), 
    ValidateLifetime=true
});


builder.Services.AddAuthorization();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/login",(LoginRequest r, IJwtTokenService jwt) => 
    r.Email == "dev@martech.com" && r.Password=="Senha@123" ? 
    Results.Ok(new{token=jwt.CreateToken(r.Email)}) : 
    Results.Unauthorized());

var orders = app.MapGroup("/api/orders").RequireAuthorization();

orders.MapPost("/",async(CreateOrderCommand c,ISender s,CancellationToken ct) =>
{ 
    var order = await s.Send(c,ct); 
    return Results.Created($"/api/orders/{order.Id}",order);
});
    
orders.MapGet("/",async(int page,int pageSize,ISender s,CancellationToken ct) => 
    Results.Ok(await s.Send(new GetOrdersQuery(page,pageSize),ct)));

orders.MapGet("/{id:guid}",async(Guid id,ISender s,CancellationToken ct) =>
{
    try
    {
        return Results.Ok(await s.Send(new GetOrderByIdQuery(id),ct));
    }
    catch(KeyNotFoundException)
    {
        return Results.NotFound();
    }
});

orders.MapPatch("/{id:guid}/cancel",async(Guid id,ISender s,CancellationToken ct) =>
{  
    try
    {
        await s.Send(new CancelOrderCommand(id),ct);
        return Results.NoContent();
    }
    catch(KeyNotFoundException)
    {
        return Results.NotFound();
    }
});

app.Run();