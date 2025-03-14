﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

        var service = scope.ServiceProvider.GetService(ValidatorType)!;

        foreach (var arg in context.ActionArguments)
        {
            if (arg.Value!.GetType() == ModelType)
            {
                MethodInfo mi = ValidatorType.GetMethod("Validate")!;

                ValidationResult? validationResult = mi!.Invoke(service, new object[] { arg.Value! }) as ValidationResult;

                if (validationResult is not null && !validationResult.IsValid)
                {
                    context.Result = new ObjectResult(validationResult.Errors) { StatusCode = 400, };
                }
            }
        }
    }
}