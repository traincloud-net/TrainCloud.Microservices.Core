using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Extensions.Swagger;

public class SwaggerOptions
{
    public const string Position = "Swagger";

    public string Title { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public string TermsOfServiceUri { get; set; } = String.Empty;

    public string ContactName { get; set; } = String.Empty;

    public string ContactEmail { get; set; } = String.Empty;

    public string LicenseName { get; set; } = String.Empty;

    public string LicenseUri { get; set; } = String.Empty;
}
