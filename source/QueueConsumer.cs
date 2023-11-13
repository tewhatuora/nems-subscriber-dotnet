using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;
using System.Text;

namespace GuaranteedSubscriber;

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

        var sessionProperties = settings.ToSessionProperties();
        
        InternalRun(sessionProperties, settings.QueueName, context);
    }

    public void Run(SessionProperties properties, string queueName, IContext context)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(properties, nameof(properties));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        InternalRun(properties, queueName, context);
    }

    private void InternalRun(SessionProperties sessionProperties, string queueName, IContext context)
    {
        // Connect to Solace.
        _logger.LogInformation("Connecting as {username}@{vpnName} on {host}...", sessionProperties.UserName, sessionProperties.VPNName, sessionProperties.Host);

        _session = context.CreateSession(sessionProperties, null, null);

        var result = _session.Connect();
        if (result is not ReturnCode.SOLCLIENT_OK)
        {
            _logger.LogError("Failed to connect to Solace. Reason: {result}", result);
            return;
        }

        _logger.LogInformation("Successfully connected to Solace!");

        // Attempt to connect to the specified queue.
        _logger.LogInformation("Attempting to connect to queue '{queueName}'", queueName);
        _queue = ContextFactory.Instance.CreateQueue(queueName);

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
            throw new Exception($"Failed to connect to queue '{queueName}', does queue exist?", ex);
        }

        while (true)
        {
            _logger.LogInformation("Waiting for message in queue '{queueName}'...", queueName);

            _eventWaitHandle.WaitOne();
        }
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
