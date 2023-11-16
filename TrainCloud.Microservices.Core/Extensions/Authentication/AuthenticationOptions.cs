using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCloud.Microservices.Core.Extensions.Authentication;

public class AuthenticationOptions
{
    public const string Position = "Identity:Token";

    public string IssuerSigningKey { get; set; } = String.Empty;

    public string ValidIssuer { get; set; } = String.Empty;

    public string ValidAudience { get; set; } = String.Empty;
}
