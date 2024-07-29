using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Services;

/// <summary>
/// Base for all service classes in the Microserivces which access a database
/// </summary>
/// <typeparam name="TService">The inheriting Service type</typeparam>
/// <typeparam name="TDbContext">The DbContext, used in the inheriing servie</typeparam>
public abstract class DataService<TService, TDbContext> : AbstractService<TService> where TDbContext : DbContext
{
    protected TDbContext DbContext { get; init; }

    protected IDistributedCache Cache { get; init; }

    protected DataService(IConfiguration configuration,
                          ILogger<TService> logger,
                          TDbContext dbContext,
                          IDistributedCache cache)
                          : base(configuration, logger)
    {
        DbContext = dbContext;
        Cache = cache;
    }
}