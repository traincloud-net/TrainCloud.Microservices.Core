using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace TrainCloud.Microservices.Core.Extensions.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddTrainCloudSwagger(this IServiceCollection services, SwaggerOptions options)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.EnableAnnotations();
            swaggerGenOptions.ExampleFilters();

            List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
            xmlFiles.ForEach(xmlFile => swaggerGenOptions.IncludeXmlComments(xmlFile, true));

            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = options.Title,
                Description = options.Description,
                TermsOfService = new Uri(options.TermsOfService),
                Contact = new OpenApiContact
                {
                    Name = options.ContactName,
                    Email = options.ContactEmail,
                    Url = new Uri("https://github.com/traincloud-net/")
                },
                License = new OpenApiLicense
                {
                    Name = options.LicenseName,
                    Url = new Uri(options.LicenseUrl)
                },
                Version = "v1"
            });

            //swaggerGenOptions.DescribeAllEnumsAsStrings();

            swaggerGenOptions.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
            swaggerGenOptions.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

            // Include 'SecurityScheme' to use JWT Authentication
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **ONLY** your JWT Bearer token in the textbox below!",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            swaggerGenOptions.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

        });
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        return services;
    }

    public static IApplicationBuilder UseTrainCloudSwagger(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseSwagger();
        applicationBuilder.UseSwaggerUI();
        return applicationBuilder;
    }

}
