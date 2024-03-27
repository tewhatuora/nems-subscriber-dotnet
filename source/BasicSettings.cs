using System.ComponentModel;
using SolaceSystems.Solclient.Messaging;
namespace GuaranteedSubscriber;

public class BasicSettings
{
    public required string Host { get; set; }
    public required string VPNName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public int DefaultReconnectRetries { get; set; } = 3;

    public SessionProperties setProperties()
    {
        ArgumentException.ThrowIfNullOrEmpty(Host, nameof(Host));
        ArgumentException.ThrowIfNullOrEmpty(VPNName, nameof(VPNName));
        ArgumentException.ThrowIfNullOrEmpty(UserName, nameof(UserName));

        if (DefaultReconnectRetries < 0)
        {
            throw new ArgumentException($"{nameof(DefaultReconnectRetries)} cannot be less than 0.");
        }

        var properties = new SessionProperties
        {
            Host = Host,
            VPNName = VPNName,
            UserName = UserName,
            Password = Password,
            ReconnectRetries = DefaultReconnectRetries,
            AuthenticationScheme = AuthenticationSchemes.BASIC,
            SSLValidateCertificate = false,
        };

        return properties;
    }

}