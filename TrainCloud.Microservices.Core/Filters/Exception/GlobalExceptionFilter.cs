using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TrainCloud.Microservices.Core.Filters.Exception;

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    private IWebHostEnvironment WebHostEnvironment { get; init; }

    private ILogger<GlobalExceptionFilter> Logger { get; init; }

    public GlobalExceptionFilter(IWebHostEnvironment webHostEnvironment, ILogger<GlobalExceptionFilter> logger)
    {
        WebHostEnvironment = webHostEnvironment;
        Logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        base.OnException(context);

        Logger.LogCritical(context.Exception.Message, context.Exception);
        Logger.LogCritical(context.Exception.StackTrace, context.Exception);

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