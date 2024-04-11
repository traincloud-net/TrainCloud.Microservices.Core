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
    public static WebApplicationBuilder AddTrainCloudDbContext<TDbContext, TDbContextImplementation>(this WebApplicationBuilder webApplicationBuilder) 
        where TDbContext : DbContext
        where TDbContextImplementation : TDbContext
    {
        webApplicationBuilder.Services.AddDbContext<TDbContext, TDbContextImplementation>();

        return webApplicationBuilder;
    }

    public static async Task<WebApplication> MigrateTrainCloudDatabaseAsync<TDbContext>(this WebApplication webApplication) where TDbContext : DbContext
    {
        if (webApplication.Environment.EnvironmentName.Equals("Production"))
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
