using SolaceSystems.Solclient.Messaging;

namespace SparkHealthSolace;

public class SolaceConfig
    : IDisposable
{
    private readonly IContext _context;

    public SolaceConfig()
    {
        var properties = new ContextFactoryProperties
        {
            SolClientLogLevel = SolLogLevel.Warning,
        };
        properties.LogToConsoleError();
        ContextFactory.Instance.Init(properties);

        _context = ContextFactory.Instance.CreateContext(new ContextProperties(), null);
    }

    public IContext Context => _context;

    public void Dispose()
    {
        _context.Dispose();
        
        // Should only be run at end of program.
        ContextFactory.Instance.Cleanup();
    }
}
