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

        System.Exception ex = context.Exception;

        Logger.LogCritical(context.Exception.Message, context.Exception);

        if (WebHostEnvironment.IsDevelopment())
        {
            var x = new { Message = context.Exception.Message, StackTrace = context.Exception.StackTrace };
            context.Result = new ObjectResult(x) { StatusCode = 500, };
        }
        else
        {
            context.Result = new StatusCodeResult(500);
        }

        context.ExceptionHandled = true;
    }
}