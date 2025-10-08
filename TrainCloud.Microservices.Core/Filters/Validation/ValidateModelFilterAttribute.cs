using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace TrainCloud.Microservices.Core.Filters.Validation;

/// <summary>
/// Apply FluentValidation to Controller endpoints 
/// Return Error 400 if model is invalid
/// Works only for [FromBody] Parametes
/// </summary>
/// <example>/// 
/// [HttpPost("Car")]
/// [Authorize(Policy = "Registry.Cars.Post")]
/// ...
/// [ValidateModelFilter(typeof(IValidator<PostCarModel>), typeof(PostCarModel))]
/// public async Task<IActionResult> PostAsync([FromBody] PostCarModel postModel)
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ValidateModelFilterAttribute : ActionFilterAttribute
{
    private Type ValidatorType { get; init; }

    private Type ModelType { get; init; }

    public ValidateModelFilterAttribute(Type validatorType, Type modelType)
    {
        ValidatorType = validatorType;
        ModelType = modelType;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        using IServiceScope scope = context.HttpContext.RequestServices.CreateScope();

        object validatorService = scope.ServiceProvider.GetService(ValidatorType)!;

        IEnumerable<object?>? argumentValues = context.ActionArguments.Select(arg => arg.Value);

        foreach (object? argumentValue in argumentValues)
        {
            if (argumentValue is not null
                && argumentValue.GetType() == ModelType)
            {
                MethodInfo mi = ValidatorType.GetMethod("Validate")!;

                object[] parameters = { argumentValue! };

                ValidationResult? validationResult = mi!.Invoke(validatorService, parameters) as ValidationResult;

                if (validationResult is not null && !validationResult.IsValid)
                {
                    context.Result = new ObjectResult(validationResult.Errors) { StatusCode = 400, };
                }
            }
        }
    }
}