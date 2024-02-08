using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Extensions.Authorization;

public static class AuthorizationExtension
{
    public static IServiceCollection AddTrainCloudAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        //services.AddAuthorization(authorizationOptions =>
        //{
        //    authorizationOptions.AddPolicy("CarLists", authorizationPolicyBuilder =>
        //    {
        //        authorizationPolicyBuilder.RequireAssertion(authorizationHandleContext =>
        //        {
        //            return true;
        //        });
        //    });
        //    authorizationOptions.AddPolicy("FaulReports", authorizationPolicyBuilder =>
        //    {
        //        authorizationPolicyBuilder.RequireAssertion(authorizationHandleContext =>
        //        {
        //            return true;
        //        });
        //    });
        //    authorizationOptions.AddPolicy("ForcedBrakings", authorizationPolicyBuilder =>
        //    {
        //        authorizationPolicyBuilder.RequireAssertion(authorizationHandleContext =>
        //        {
        //            return true;
        //        });
        //    });
        //});

        return services;
    }
}
