using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace TrainCloud.Microservices.Core.Extensions.Kestrel;

public static class KestrelExtensions
{
    public static IWebHostBuilder ConfigureTrainCloudKestrel(this IWebHostBuilder builder, int applicationPort, 
                                                             bool useHttps,
                                                             bool useHealth,
                                                             bool useMetrics)
    {
        builder.ConfigureKestrel(options =>
        {
            // Application Development/Tests/Production
            options.Listen(System.Net.IPAddress.Any, applicationPort, listenOptions =>
            {
                if (useHttps)
                {
                    listenOptions.UseHttps();
                }

                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            if(useHealth)
            {
                // For MapHealthChecks http://*:8080/health
                options.Listen(System.Net.IPAddress.Any, 8080, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            }

            if (useMetrics)
            {
                // For Prometheus metrics http://*:9090/metrics
                options.Listen(System.Net.IPAddress.Any, 9090, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            }
        });

        return builder;
    }
}