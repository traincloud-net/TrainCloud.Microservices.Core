using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Extensions.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddTrainCloudAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddCookie(IdentityConstants.ApplicationScheme)
        .AddJwtBearer(jwtBearerOptions =>
                           {
                               string jwtKey = configuration.GetSection("Identity").GetSection("Token").GetValue<string>("JwtSecurityKey")!;
                               if (jwtKey is not null)
                               {
                                   jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                                   {
                                       ValidateIssuer = false,
                                       ValidateAudience = false,
                                       ValidateLifetime = true,
                                       ValidateIssuerSigningKey = false,
                                       ValidIssuer = configuration.GetSection("Identity").GetSection("Token").GetValue<string>("JwtIssuer"),
                                       ValidAudience = configuration.GetSection("Identity").GetSection("Token").GetValue<string>("JwtAudience"),
                                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                                   };
                               }
                           });
        return services;
    }
}
