using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;

namespace GuaranteedSubscriber;

internal static class SolLogLevelExtensions
{
    public static LogLevel ToExtensionsLogLevel(this SolLogLevel level)
    {
        return level switch
        {
            SolLogLevel.Debug => LogLevel.Debug,
            SolLogLevel.Info => LogLevel.Information,
            SolLogLevel.Warning => LogLevel.Warning,
            SolLogLevel.Error => LogLevel.Error,
            SolLogLevel.Alert => LogLevel.Information,
            SolLogLevel.Critical => LogLevel.Critical,
            SolLogLevel.Emergency => LogLevel.Critical,
            _ => LogLevel.Information
        };
    }
}
