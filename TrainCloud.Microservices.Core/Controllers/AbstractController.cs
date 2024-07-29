using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrainCloud.Microservices.Core.Controllers;

/// <summary>
/// A basic controller all TrainCloud controllers inherit from.
/// </summary>
/// <typeparam name="TController">The inheriting Controller type</typeparam>
public abstract class AbstractController<TController> : ControllerBase
{
    protected IWebHostEnvironment WebHostEnvironment { get; init; }

    protected IHttpContextAccessor HttpContextAccessor { get; init; }

    protected IConfiguration Configuration { get; init; }

    protected ILogger<TController> Logger { get; init; }

    /// <summary>
    /// Indicated if the current request is authenticated or not
    /// </summary>
    protected bool IsAnonymous
    {
        get
        {
            return (CurrentUserId == Guid.Empty);
        }
    }

    /// <summary>
    /// The current UserId in this request
    /// </summary>
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

    /// <summary>
    /// 
    /// From JWT Data
    /// </summary>
    protected string? CurrentUserDocumentId
    {
        get
        {
            string? userDocumentId = HttpContext?.User?.FindFirst("DocumentId")?.Value;

            return userDocumentId;
        }
    }

    protected string? CurrentUserEMail
    {
        get
        {
            string? userEMail = HttpContext?.User?.FindFirst("EMail")?.Value;

            return userEMail;
        }
    }

    protected string? CurrentUserName
    {
        get
        {
            string? userName = HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            return userName;
        }
    }

    protected List<string>? CurrentRoles
    {
        get
        {
            List<string>? roles = HttpContext?.User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return roles;
        }
    }

    protected Guid? CurrentTenantId
    {
        get
        {
            string? tenantId = HttpContext?.User?.FindFirst("TenantId")?.Value;
            if (string.IsNullOrEmpty(tenantId))
            {
                return default;
            }

            return Guid.Parse(tenantId);
        }
    }

    protected string? CurrentTenantName
    {
        get
        {
            string? tenantName = HttpContext?.User?.FindFirst("TenantName")?.Value;

            return tenantName;
        }
    }

    protected string? CurrentTenantEMail
    {
        get
        {
            string? tenantEmail = HttpContext?.User?.FindFirst("TenantEMail")?.Value;

            return tenantEmail;
        }
    }

    protected string? CurrentTenantVKM
    {
        get
        {
            string? vkm = HttpContext?.User?.FindFirst("VKM")?.Value;

            return vkm;
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

    /// <summary>
    /// Returns a HTTP Status 201 - Locked, with the created object.
    /// </summary>
    /// <returns>HTTP Status 201 with response body</returns>
    protected IActionResult Created(object? model = null)
    {
        return StatusCode(201, model);
    }

    /// <summary>
    /// Returns a HTTP Status 401 - NotAuthorized, without result.
    /// </summary>
    /// <returns>HTTP Status 401</returns>
    protected IActionResult NotAuthorized()
    {
        return StatusCode(401);
    }

    /// <summary>
    /// Returns a HTTP Status 403 - Forbidden, without result.
    /// </summary>
    /// <returns>HTTP Status 403</returns>
    protected IActionResult Forbidden()
    {
        return StatusCode(403);
    }

    /// <summary>
    /// Returns a HTTP Status 415 - UnsupportedMediaType, without result.
    /// </summary>
    /// <returns>HTTP Status 415</returns>
    protected IActionResult UnsupportedMediaType()
    {
        return StatusCode(415);
    }

    /// <summary>
    /// Returns a HTTP Status 423 - Locked, without result.
    /// </summary>
    /// <returns>HTTP Status 423</returns>
    protected IActionResult Locked()
    {
        return StatusCode(423);
    }
}
