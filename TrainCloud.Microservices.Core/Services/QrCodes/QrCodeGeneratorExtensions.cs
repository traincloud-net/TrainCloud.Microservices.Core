using Microsoft.Extensions.DependencyInjection;

namespace TrainCloud.Microservices.Core.Services.QrCodes;

public static class QrCodeGeneratorExtensions
{
    /// <summary>
    /// Adds the QrCodeGenerator to the Microservice DI container.
    /// Singleton
    /// https://cloud.google.com/dotnet/docs/reference/Google.Cloud.PubSub.V1/latest#performance-considerations-and-default-settings
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTrainCloudQrCodeGenerator(this IServiceCollection services)
    {
        services.AddScoped<IQrCodeGenerator, QrCodeGenerator>();
        return services;
    }
}
