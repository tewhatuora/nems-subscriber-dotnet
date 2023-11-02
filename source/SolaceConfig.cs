using SolaceSystems.Solclient.Messaging;

namespace SparkHealthSolace;

public class SolaceConfig
    : IDisposable
{ 
    public SolaceConfig()
    {
        var properties = new ContextFactoryProperties
        {
            SolClientLogLevel = SolLogLevel.Warning,
        };
        properties.LogToConsoleError();

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
}
