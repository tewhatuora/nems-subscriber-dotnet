using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;

namespace GuaranteedSubscriber;

public class NEMSConfig
    : IDisposable
{
    private readonly ILogger _logger;

    public NEMSConfig(ILogger logger)
    {
        _logger = logger;

        var properties = new ContextFactoryProperties
        {
            SolClientLogLevel = SolLogLevel.Warning,
            LogDelegate = LogDelegate,
        };
        
        ContextFactory.Instance.Init(properties);
    }

    public IContext CreateContext()
    {
        var context = ContextFactory.Instance.CreateContext(new ContextProperties(), null); 
        return context;
    }

    public void Dispose()
    {
        ContextFactory.Instance.Cleanup();
    }

    private void LogDelegate(SolLogInfo logInfo)
    {
        _logger.Log(logInfo.LogLevel.ToExtensionsLogLevel(), logInfo.LogException, logInfo.LogMessage); ;
    }
}
