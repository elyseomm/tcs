using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace OrderManagement.Application.Behaviors;
public sealed class LoggingBehavior<TRequest,TResponse>(ILogger<LoggingBehavior<TRequest,TResponse>> logger) : IPipelineBehavior<TRequest,TResponse> where TRequest:notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) 
    { 
        var sw = Stopwatch.StartNew(); 
        try { 
            var response = await next(ct);
            logger.LogInformation("{Request} completed in {Elapsed}ms. Response: {@Response}",
                typeof(TRequest).Name, sw.ElapsedMilliseconds, response); 
            return response;
        } 
        catch(Exception ex)
        {
            logger.LogError(ex, "{Request} failed in {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds); 
            throw;
        } 
    } 
}
