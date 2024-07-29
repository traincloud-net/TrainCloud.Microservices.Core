namespace TrainCloud.Microservices.Core.Extensions.Authentication;

public class AuthenticationOptions
{
    public const string Position = "Identity:Token";

    public string IssuerSigningKey { get; set; } = String.Empty;

    public string ValidIssuer { get; set; } = String.Empty;

    public string ValidAudience { get; set; } = String.Empty;
}
