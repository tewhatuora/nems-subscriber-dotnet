using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;
using Newtonsoft.Json;

namespace GuaranteedSubscriber;
public class EventLoader
{
    private readonly ILogger<EventLoader> _logger;

    public EventLoader()
    {
        _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole(); // Add other logging providers as needed
        }).CreateLogger<EventLoader>();
    }

    public void ProcessEvent(IMessage message)
    {
        try
        {
            _logger.LogInformation("Message topic: {content}", message.Destination.Name);
            _logger.LogInformation("Message content: {content}", Encoding.ASCII.GetString(message.BinaryAttachment));
            _logger.LogInformation("Header source: {header}", message.UserPropertyMap.GetString("source"));
            _logger.LogInformation("Test: {details}", message.ADMessageId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to process message '{message.ReplicationGroupMessageId}', message placed remains on queue", ex);
        }
    }
}