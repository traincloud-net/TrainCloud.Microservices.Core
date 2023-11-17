using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Services;

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