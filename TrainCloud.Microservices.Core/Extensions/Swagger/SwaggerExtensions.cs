using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace TrainCloud.Microservices.Core.Extensions.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddTrainCloudSwagger(this IServiceCollection services)
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
                Title = "Title",// webApplicationBuilder.Configuration.GetSection("Swagger").GetValue<string>("Title"),
                Description = "Description",// webApplicationBuilder.Configuration.GetSection("Swagger").GetValue<string>("Description"),
                TermsOfService = new Uri("https://traincloud.net/Terms"),
                Contact = new OpenApiContact
                {
                    Name = "TrainCloud",
                    Email = "mail@traincloud.net"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://TrainCloud/LICENSE.txt")
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
