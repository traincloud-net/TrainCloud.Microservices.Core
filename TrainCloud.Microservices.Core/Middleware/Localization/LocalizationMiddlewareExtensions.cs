using Microsoft.AspNetCore.Builder;

namespace TrainCloud.Microservices.Core.Middleware.Localization;

public static class LocalizationMiddlewareExtensions
{
    public static WebApplication UseTrainCloudLocalization(this WebApplication webApplication)
    {
        webApplication.UseMiddleware<LocalizationMiddleware>();
        return webApplication;
    }
}
