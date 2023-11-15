using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Services;

public abstract class AbstractService<TService>
{
    protected IConfiguration Configuration { get; init; }

    protected ILogger<TService> Logger { get; init; }

    protected AbstractService(IConfiguration configuration,
                              ILogger<TService> logger)
    {
        Configuration = configuration;
        Logger = logger;
    }
}