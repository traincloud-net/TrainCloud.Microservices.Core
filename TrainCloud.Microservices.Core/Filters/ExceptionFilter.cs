using Microsoft.AspNetCore.Mvc.Filters;

namespace TrainCloud.Microservices.Core.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        System.Diagnostics.Debugger.Break();
    }
}
