using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Controllers;

public abstract class AbstractController<TController> : ControllerBase
{
    protected IWebHostEnvironment WebHostEnvironment { get; init; }

    protected IHttpContextAccessor HttpContextAccessor { get; init; }

    protected IConfiguration Configuration { get; init; }

    protected ILogger<TController> Logger { get; init; }

    protected Guid? CurrentTenantId
    {
        get
        {
            string? tenantId = HttpContext?.User?.FindFirst(ClaimTypes.UserData)?.Value;
            if (string.IsNullOrEmpty(tenantId))
            {
                return default;
            }

            return Guid.Parse(tenantId);
        }
    }

    protected Guid CurrentUserId
    {
        get
        {
            string? userId = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Guid.Empty;
            }

            return Guid.Parse(userId);
        }
    }

    protected string CurrentRole
    {
        get
        {
            string? role = HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
            {
                return string.Empty;
            }

            return role;
        }
    }

    protected AbstractController(IWebHostEnvironment webHostEnvironment,
                                 IHttpContextAccessor httpContextAccessor,
                                 IConfiguration configuration,
                                 ILogger<TController> logger)
    {
        WebHostEnvironment = webHostEnvironment;
        HttpContextAccessor = httpContextAccessor;
        Configuration = configuration;
        Logger = logger;
    }

    protected ActionResult Created(object? model = null)
    {
        return StatusCode(201, model);
    }

    protected ActionResult NotAuthorized()
    {
        return StatusCode(401);
    }

    protected ActionResult Forbidden()
    {
        return StatusCode(403);
    }

    protected ActionResult UnsupportedMediaType()
    {
        return StatusCode(415);
    }

    protected ActionResult InternalServerError(Exception ex)
    {
        Logger.LogError(ex.Message, ex);

        if (WebHostEnvironment.IsDevelopment())
        {
            return StatusCode(500, ex.Message);
        }

        return StatusCode(500);
    }
}