using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Database;
public static class DbContextExtensions
{
    public static WebApplicationBuilder AddTrainCloudDbContext<TDbContext>(this WebApplicationBuilder webApplicationBuilder) where TDbContext : DbContext
    {
        webApplicationBuilder.Services.AddDbContext<TDbContext>(options =>
        {
            switch (webApplicationBuilder.Environment.EnvironmentName)
            {
                case "Test":
                    options.EnableDetailedErrors(true);
                    options.UseInMemoryDatabase(webApplicationBuilder.Environment.ApplicationName);
                    break;
                case "Development":
                case "Production":
                    options.EnableDetailedErrors(true);
                    options.UseNpgsql(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
                    break;
            }
        });

        return webApplicationBuilder;
    }

    public static async Task<WebApplication> MigrateTrainCloudDatabaseAsync<TDbContext>(this WebApplication webApplication) where TDbContext : DbContext
    {
        if (!webApplication.Environment.EnvironmentName.Equals("Test"))
        {
            using (IServiceScope scope = webApplication.Services.CreateScope())
            {
                TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                await dbContext.Database.MigrateAsync();
            }
        }

        return webApplication;
    }
}
