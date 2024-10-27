using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TrainCloud.Microservices.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TGenericDbContext"></typeparam>
/// <typeparam name="TPostgresDbContext"></typeparam>
/// <typeparam name="TSqliteDbContext"></typeparam>
public sealed class TrainCloudDbContextFactory<TGenericDbContext, TPostgresDbContext, TSqliteDbContext> : IDbContextFactory<TGenericDbContext>
    where TGenericDbContext : DbContext
    where TPostgresDbContext : TGenericDbContext
    where TSqliteDbContext : TGenericDbContext
{
    private IConfiguration Configuration { get; init; }

    public TrainCloudDbContextFactory(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public TGenericDbContext CreateDbContext()
    {
        string databaseProvider = Configuration.GetValue<string>("Database:Provider")!;
        TGenericDbContext? dbContext = null;
        switch (databaseProvider.ToLower())
        {
            case "postgresql":
                dbContext = (TPostgresDbContext)Activator.CreateInstance(typeof(TPostgresDbContext), Configuration)!;
                break;
            case "sqlite":
                dbContext = (TSqliteDbContext)Activator.CreateInstance(typeof(TSqliteDbContext), Configuration)!;
                break;
            default:
                throw new ArgumentException($"Database:Provider: {databaseProvider} is invalid");
        }

        return dbContext!;
    }
}