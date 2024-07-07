using Microsoft.AspNetCore.Builder;

namespace TrainCloud.Microservices.Core.Middleware.LoadBalancing;

public static class LoadBalancingMiddlewareExtensions
{
    public static WebApplication UseTrainCloudLoadBalancing(this WebApplication webApplication)
    {
        webApplication.UseMiddleware<LoadBalancingMiddleware>();
        return webApplication;
    }
}
