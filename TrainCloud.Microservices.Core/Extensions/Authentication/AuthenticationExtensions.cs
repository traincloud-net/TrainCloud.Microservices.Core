using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Extensions.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddTrainCloudAuthentication(this IServiceCollection services)
    {

        return services;
    }
}
