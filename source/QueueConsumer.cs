using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;
using System.Text;

namespace SparkHealthSolace;

public class QueueConsumer
    : IDisposable
{
    private readonly ILogger _logger;
    private readonly EventWaitHandle _eventWaitHandle;

    private ISession? _session;
    private IQueue? _queue;
    private IFlow? _flow;

    public QueueConsumer(ILogger<QueueConsumer> logger)
    {
        _logger = logger;
        _eventWaitHandle = new AutoResetEvent(false);
    }

    public void Run(QueueConsumerSettings settings, IContext context)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        settings.Validate();

        var sessionProperties = new SessionProperties
        {
            Host = settings.Host,
            VPNName = settings.VPNName,
            UserName = settings.Username,
            Password = settings.Password,
            ReconnectRetries = settings.DefaultReconnectRetries,
            SSLValidateCertificate = false,
        };

        // Connect to Solace.
        _logger.LogInformation("Connecting as {username}@{vpnName} on {host}...", settings.Username, settings.VPNName, settings.Host);

        _session = context.CreateSession(sessionProperties, null, null);

        var result = _session.Connect();
        if (result is not ReturnCode.SOLCLIENT_OK)
        {
            _logger.LogError("Failed to connect to Solace. Reason: {result}", result);
            return;
        }

        _logger.LogInformation("Successfully connected to Solace!");

        // Attempt to connect to the specified queue.
        _logger.LogInformation("Attempting to connect to queue '{queueName}'", settings.QueueName);
        _queue = ContextFactory.Instance.CreateQueue(settings.QueueName);

        var flowProperties = new FlowProperties
        {
            AckMode = MessageAckMode.ClientAck,
        };

        try
        {
            _flow = _session.CreateFlow(flowProperties, _queue, null, HandleMessageEvent, HandleFlowEvent);
            _flow.Start();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to connect to queue '{settings.QueueName}', does queue exist?", ex);
        }

        _logger.LogInformation("Waiting for message in queue '{queueName}'...", settings.QueueName);

        _eventWaitHandle.WaitOne();
    }

    private void HandleMessageEvent(object? source, MessageEventArgs args)
    {
        _logger.LogInformation("Message recieved");

        using var message = args.Message;

        _logger.LogInformation("Message content: {content}", Encoding.ASCII.GetString(message.BinaryAttachment));
        _flow?.Ack(message.ADMessageId);
        _eventWaitHandle.Set();
    }

    private void HandleFlowEvent(object? source, FlowEventArgs args)
    {
        _logger.LogInformation("Recieved Flow Event '{event}' Type: '{responseCode}' Text: '{info}'", args.Event, args.ResponseCode, args.Info);
    }

    public void Dispose()
    {
        _session?.Dispose();
        _queue?.Dispose();
        _flow?.Dispose();
    }
}
