using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;
using SolaceSystems.Solclient.Messaging.SDT;

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
            string source = message.UserPropertyMap?.GetString("source") ?? throw new ArgumentNullException("source", "Invalid message, the property 'source' cannot be NULL.");
            string id = message.UserPropertyMap?.GetString("id") ?? throw new ArgumentNullException("id", "Invalid message, the property 'id' cannot be NULL.");
            string type = message.UserPropertyMap?.GetString("type") ?? throw new ArgumentNullException("type", "Invalid message, the property 'type' cannot be NULL.");
            string datacontenttype = message.UserPropertyMap?.GetString("datacontenttype") ?? throw new ArgumentNullException("datacontenttype", "Invalid message, the property 'datacontenttype' cannot be NULL.");
            string subject = message.UserPropertyMap?.GetString("subject") ?? throw new ArgumentNullException("subject", "Invalid message, the property 'subject' cannot be NULL.");
            string time = message.UserPropertyMap?.GetString("time") ?? throw new ArgumentNullException("time", "Invalid message, the property 'time' cannot be NULL.");
            ArgumentNullException.ThrowIfNull(message.BinaryAttachment, "Message has no payload");

            JsonDocument.Parse(SDTUtils.GetText(message));

            _logger.LogInformation("Message topic: {content}", message.Destination.Name);
            _logger.LogInformation("Message content: {content}", SDTUtils.GetText(message));
            _logger.LogInformation("Header source: {header}", message.UserPropertyMap.GetString("source"));
            _logger.LogInformation("Test: {details}", message.ADMessageId);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError($"Message '{message.ReplicationGroupMessageId}' is invalid : " + ex);
            throw new Exception($"REJECTED");
        }
        catch (FieldNotFoundException ex)
        {
            _logger.LogError($"Message '{message.ReplicationGroupMessageId}' is invalid, missing mandatory property : " + ex);
            throw new Exception($"REJECTED");
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Message '{message.ReplicationGroupMessageId}' is invalid JSON format " + ex);
            throw new Exception($"REJECTED " + ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Message '{message.ReplicationGroupMessageId}' is invalid : " + ex);
            throw new Exception($"FAILED");
        }
    }
}