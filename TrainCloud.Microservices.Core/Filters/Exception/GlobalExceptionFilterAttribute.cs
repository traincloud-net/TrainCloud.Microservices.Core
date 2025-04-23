using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TrainCloud.Microservices.Core.Filters.Exception;

/// <summary>
/// Global exception handler for all Microservices
/// </summary>
public sealed class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
{
    private IWebHostEnvironment WebHostEnvironment { get; init; }

    private ILogger<GlobalExceptionFilterAttribute> Logger { get; init; }

    public GlobalExceptionFilterAttribute(IWebHostEnvironment webHostEnvironment, ILogger<GlobalExceptionFilterAttribute> logger)
    {
        WebHostEnvironment = webHostEnvironment;
        Logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        base.OnException(context);

        Logger.LogCritical(context.Exception.Message);
        Logger.LogCritical(context.Exception.StackTrace);

        if (WebHostEnvironment.IsDevelopment())
        {
            dynamic responseMessage = new 
            { 
                Message = context.Exception.Message, 
                StackTrace = context.Exception.StackTrace 
            };
            context.Result = new ObjectResult(responseMessage) { StatusCode = 500, };

            Debugger.Break();
        }
        else
        {
            context.Result = new StatusCodeResult(500);
        }
        
        context.ExceptionHandled = true;
    }
}