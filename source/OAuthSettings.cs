using SolaceSystems.Solclient.Messaging;
namespace GuaranteedSubscriber;

public class OAuthSettings
{
    public required string Host { get; set; }
    public required string VPNName { get; set; }
    public required string Token { get; set; }
    public required string Issuer { get; set; }
    public AuthenticationSchemes AuthenticationScheme { get; set; } = AuthenticationSchemes.OAUTH2;
    public int DefaultReconnectRetries { get; set; } = 3;

    public SessionProperties setProperties()
    {
        ArgumentException.ThrowIfNullOrEmpty(Host, nameof(Host));
        ArgumentException.ThrowIfNullOrEmpty(VPNName, nameof(VPNName));
        ArgumentException.ThrowIfNullOrEmpty(Token, nameof(Token));

        if (DefaultReconnectRetries < 0)
        {
            throw new ArgumentException($"{nameof(DefaultReconnectRetries)} cannot be less than 0.");
        }

        var properties = new SessionProperties
        {
            Host = Host,
            VPNName = VPNName,
            OAuth2AccessToken = Token,
            OAuth2IssuerIdentifier = Issuer,
            ReconnectRetries = DefaultReconnectRetries,
            AuthenticationScheme = AuthenticationSchemes.OAUTH2,
            SSLValidateCertificate = false,
        };

        return properties;
    }

}