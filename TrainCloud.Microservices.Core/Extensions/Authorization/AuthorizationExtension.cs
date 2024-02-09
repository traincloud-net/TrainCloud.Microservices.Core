using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using TrainCloud.Models;

namespace TrainCloud.Microservices.Core.Extensions.Authorization;

public static class AuthorizationExtension
{
    public static IServiceCollection AddTrainCloudAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(authorizationOptions =>
        {
            authorizationOptions.AddPolicy("CarLists", authorizationPolicyBuilder =>
            {
                authorizationPolicyBuilder.RequireAuthenticatedUser();
                authorizationPolicyBuilder.RequireAssertion(authorizationHandleContext =>
                {
                    if (authorizationHandleContext.User.IsInRole("Administrator") ||
                       authorizationHandleContext.User.IsInRole("DataCustodian"))
                    {
                        return true;
                    }

                    if (authorizationHandleContext.User.IsInRole("TenantOwner") ||
                       authorizationHandleContext.User.IsInRole("TenantDispatcher") ||
                       authorizationHandleContext.User.IsInRole("TenantUser"))
                    {
                        Claim featureClaim = authorizationHandleContext.User.Claims.Where(c => c.Type == "Features").First();
                        Feature features = (Feature)int.Parse(featureClaim.Value);

                        if ((features & Feature.CarLists) == Feature.CarLists)
                        {
                            return true;
                        }
                    }

                    return false;
                });
            });

            authorizationOptions.AddPolicy("ForcedBrakings", authorizationPolicyBuilder =>
            {
                authorizationPolicyBuilder.RequireAuthenticatedUser();
                authorizationPolicyBuilder.RequireAssertion(authorizationHandleContext =>
                {
                    if (authorizationHandleContext.User.IsInRole("Administrator") ||
                       authorizationHandleContext.User.IsInRole("DataCustodian"))
                    {
                        return true;
                    }

                    if (authorizationHandleContext.User.IsInRole("TenantOwner") ||
                       authorizationHandleContext.User.IsInRole("TenantDispatcher") ||
                       authorizationHandleContext.User.IsInRole("TenantUser"))
                    {
                        Claim featureClaim = authorizationHandleContext.User.Claims.Where(c => c.Type == "Features").First();
                        Feature features = (Feature)int.Parse(featureClaim.Value);

                        if ((features & Feature.CarLists) == Feature.CarLists)
                        {
                            return true;
                        }
                    }

                    return false;
                });
            });

            authorizationOptions.AddPolicy("FaulReports", authorizationPolicyBuilder =>
            {
                authorizationPolicyBuilder.RequireAuthenticatedUser();
                authorizationPolicyBuilder.RequireAssertion(authorizationHandleContext =>
                {
                    if (authorizationHandleContext.User.IsInRole("Administrator") ||
                       authorizationHandleContext.User.IsInRole("DataCustodian"))
                    {
                        return true;
                    }

                    if (authorizationHandleContext.User.IsInRole("TenantOwner") ||
                       authorizationHandleContext.User.IsInRole("TenantDispatcher") ||
                       authorizationHandleContext.User.IsInRole("TenantUser"))
                    {
                        Claim featureClaim = authorizationHandleContext.User.Claims.Where(c => c.Type == "Features").First();
                        Feature features = (Feature)int.Parse(featureClaim.Value);

                        if ((features & Feature.CarLists) == Feature.CarLists)
                        {
                            return true;
                        }
                    }

                    return false;
                });
            });

            authorizationOptions.AddPolicy("Scanner", authorizationPolicyBuilder =>
            {
                authorizationPolicyBuilder.RequireAuthenticatedUser();
                authorizationPolicyBuilder.RequireRole("Administrator", "DataCustodian", "TenantOwner", "TenantDispatcher", "TenantUser", "TrainSpotter");
            });
        });

        return services;
    }
}
