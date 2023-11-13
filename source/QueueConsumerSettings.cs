using SolaceSystems.Solclient.Messaging;

namespace GuaranteedSubscriber;

public class QueueConsumerSettings
{
    public required string Host { get; set; }
    public required string VPNName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string QueueName { get; set; }
    public int DefaultReconnectRetries { get; set; } = 3;

    public SessionProperties ToSessionProperties()
    {
        ArgumentException.ThrowIfNullOrEmpty(Host, nameof(Host));
        ArgumentException.ThrowIfNullOrEmpty(VPNName, nameof(VPNName));
        ArgumentException.ThrowIfNullOrEmpty(Username, nameof(Username));
        ArgumentException.ThrowIfNullOrEmpty(Password, nameof(Password));
        ArgumentException.ThrowIfNullOrEmpty(QueueName, nameof(QueueName));

        if (DefaultReconnectRetries < 0)
        {
            throw new ArgumentException($"{nameof(DefaultReconnectRetries)} cannot be less than 0.");
        }

        var properties = new SessionProperties
        {
            Host = Host,
            VPNName = VPNName,
            UserName = Username,
            Password = Password,
            ReconnectRetries = DefaultReconnectRetries,
            SSLValidateCertificate = false,
        };

        return properties;
    }
}
