using Google.Apis.Logging;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

        if (context.Exception.InnerException is not null)
        {
            Logger.LogCritical(context.Exception.InnerException.Message, context.Exception.InnerException);
            Logger.LogCritical(context.Exception.InnerException.StackTrace, context.Exception.InnerException);
        }

        if (WebHostEnvironment.IsDevelopment())
        {
            dynamic responseMessage = new 
            { 
                Message = context.Exception.Message, 
                StackTrace = context.Exception.StackTrace 
            };
            context.Result = new ObjectResult(responseMessage) { StatusCode = 500, };
        }
        else
        {
            context.Result = new StatusCodeResult(500);
        }

        context.ExceptionHandled = true;
    }
}