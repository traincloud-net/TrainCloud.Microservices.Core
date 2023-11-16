using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TrainCloud.Microservices.Core.Extensions.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddTrainCloudAuthentication(this IServiceCollection services, AuthenticationOptions options)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                                   {
                                       jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                                       {
                                           ValidateIssuer = false,
                                           ValidateAudience = false,
                                           ValidateLifetime = true,
                                           ValidateIssuerSigningKey = false,
                                           ValidIssuer = options.ValidIssuer,
                                           ValidAudience = options.ValidAudience,
                                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey))
                                       };
                                   });

        return services;
    }
}
