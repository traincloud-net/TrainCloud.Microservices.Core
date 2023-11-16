using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Extensions.Cors;

public static class CorsExtensions
{
    public static IServiceCollection AddTrainCloudCors(this IServiceCollection services)
    {
        services.AddCors(corsOptions =>
        {
            corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
            {
                corsPolicyBuilder.AllowAnyOrigin()
                                 .SetIsOriginAllowed(allowed => true)
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
            });
        });

        return services;
    }
}
