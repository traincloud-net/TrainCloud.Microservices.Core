using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Middleware;

public abstract class AbstractMiddleware<TMiddleware>
{
    protected RequestDelegate Next { get; init; }

    protected IConfiguration Configuration { get; init; }

    protected ILogger<TMiddleware> Logger { get; init; }

    public AbstractMiddleware(RequestDelegate next,
                              IConfiguration configuration,
                              ILogger<TMiddleware> logger)
    {
        Next = next;
        Configuration = configuration;
        Logger = logger;
    }
}