using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;

namespace GuaranteedSubscriber;

public class InitConsumer : IDisposable
{
    private readonly ILogger _logger;
    private readonly EventWaitHandle _eventWaitHandle;


    private ISession? _session;
    private IQueue? _queue;
    private IFlow? _flow;
    private IConfiguration? _config;

    public InitConsumer(ILogger<InitConsumer> logger)
    {
        _logger = logger;
        _eventWaitHandle = new AutoResetEvent(false);
    }

    public void Connect(IConfiguration configuration, IContext context)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        _config = configuration;
        var queueName = _config["NEMSProperties:QueueName"] ?? string.Empty;
        var connType = _config["NEMSProperties:ConnectionType"] ?? string.Empty;

        if (connType.Equals("BASIC"))
        {
            var sessionProperties = new NEMSConnection().BasicConnection(_config);
            ConnectQueue(sessionProperties, queueName, context);
        }
        else
        {
            var sessionProperties = new NEMSConnection().OAuthConnection(_config);
            ConnectQueue(sessionProperties, queueName, context);
        }

    }

    private void ConnectQueue(SessionProperties sessionProperties, string queueName, IContext context)
    {
        // Connect to NEMS.
        _logger.LogInformation("Connecting as {vpnName} on {host}...", sessionProperties.VPNName, sessionProperties.Host);

        _session = context.CreateSession(sessionProperties, null, HandleSessionEvent);

        var result = _session.Connect();
        if (result is not ReturnCode.SOLCLIENT_OK)
        {
            _logger.LogError("Failed to connect to NEMS. Reason: {result}", result);
            return;
        }

        _logger.LogInformation("Successfully connected to NEMS!");

        // Attempt to connect to the specified queue.
        _logger.LogInformation("Attempting to connect to queue '{queueName}'", queueName);
        _queue = ContextFactory.Instance.CreateQueue(queueName);

        var flowProperties = new FlowProperties
        {
            AckMode = MessageAckMode.ClientAck,
            RequiredOutcomeFailed = true,
            RequiredOutcomeRejected = true
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

        try
        {
            new EventLoader().ProcessEvent(message);
            _flow?.Settle(message.ADMessageId, MessageOutcome.Accepted);
        }
        catch (Exception ex)
        {

            if (ex?.Message == "FAILED")
            {
                _logger.LogInformation("Message : " + message.ADMessageId + " has FAILED and placed back on the queue for reprocessing");
                _flow?.Settle(message.ADMessageId, MessageOutcome.Failed);
            }
            else
            {
                _logger.LogInformation("Message : " + message.ADMessageId + " has been REJECTED and dropped form the queue");
                _flow?.Settle(message.ADMessageId, MessageOutcome.Rejected);
            }

        }

        _eventWaitHandle.Set();
    }

    private void HandleFlowEvent(object? source, FlowEventArgs args)
    {
        _logger.LogInformation("Recieved Flow Event '{event}' Type: '{responseCode}' Text: '{info}'", args.Event, args.ResponseCode, args.Info);
    }

    private void HandleSessionEvent(object? source, SessionEventArgs args)
    {

        if (args.Event.Equals(SessionEvent.Reconnecting))
        {

            _logger.LogInformation("Reconnecting - Renewing Token");
            NEMSConnection conn = new NEMSConnection();

            if (_session != null && _config != null)
            {
                _session.ModifyProperty(SessionProperties.PROPERTY.OAuth2AccessToken, conn.RenewToken(_config));
            }
            else
            {
                _logger.LogInformation("Renewing Token FAILED - Session and / or configuration NULL");
            }
        }
        _logger.LogInformation("Recieved Session Event '{event}' Type: '{responseCode}' Text: '{info}'", args.Event, args.ResponseCode, args.Info);
    }

    public void Dispose()
    {
        _session?.Dispose();
        _queue?.Dispose();
        _flow?.Dispose();
    }


}
