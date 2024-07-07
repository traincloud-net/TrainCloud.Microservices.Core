using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace TrainCloud.Microservices.Core.Middleware.Localization;

public class LocalizationMiddleware : AbstractMiddleware<LocalizationMiddleware>
{
    public LocalizationMiddleware(RequestDelegate next,
                                  IConfiguration configuration,
                                  ILogger<LocalizationMiddleware> logger)
        : base(next, configuration, logger)
    {

    }

    public Task Invoke(HttpContext httpContext)
    {
        string? acceptLanguage = httpContext.Request.Headers["Accept-Language"];

        string requestCulture = "en";
        if (!string.IsNullOrEmpty(acceptLanguage))
        {
            requestCulture = acceptLanguage.Substring(0, 2);
        }

        CultureInfo culture = new(requestCulture);

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        return Next(httpContext);
    }
}
