using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace TrainCloud.Microservices.Core.Middleware.LoadBalancing;

public sealed class LoadBalancingMiddleware : AbstractMiddleware<LoadBalancingMiddleware>
{
    public LoadBalancingMiddleware(RequestDelegate next,
                                   IConfiguration configuration,
                                   ILogger<LoadBalancingMiddleware> logger)
        : base(next, configuration, logger)
    {

    }

    public Task Invoke(HttpContext httpContext)
    {
        string trainCloudRegion = Environment.GetEnvironmentVariable("TRAINCLOUD_REGION") ?? "unknown";
        string trainEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
        httpContext.Response.Headers.Append("TrainCloud-Service-Region", trainCloudRegion);
        httpContext.Response.Headers.Append("TrainCloud-Service-Environment", trainEnvironment);

        return Next(httpContext);
    }
}
