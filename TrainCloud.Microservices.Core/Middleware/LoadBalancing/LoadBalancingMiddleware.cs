using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace TrainCloud.Microservices.Core.Middleware.LoadBalancing;

public class LoadBalancingMiddleware : AbstractMiddleware<LoadBalancingMiddleware>
{
    public LoadBalancingMiddleware(RequestDelegate next,
                                   IConfiguration configuration,
                                   ILogger<LoadBalancingMiddleware> logger)
        : base(next, configuration, logger)
    {

    }

    public Task Invoke(HttpContext httpContext)
    {
        string hostName = httpContext.Request.Host.HasValue ? httpContext.Request.Host.Host : "unknown";
        StringValues instanceName = new StringValues(new[] { hostName });
        httpContext.Response.Headers.Append("TrainCloud-Service", instanceName);

        return Next(httpContext);
    }
}
