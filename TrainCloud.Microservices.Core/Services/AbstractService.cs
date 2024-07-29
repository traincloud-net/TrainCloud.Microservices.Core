using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Services;

/// <summary>
/// Base for all service classes in the Microserivces
/// </summary>
/// <typeparam name="TService">The inheriting Service type</typeparam>
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