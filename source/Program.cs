using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GuaranteedSubscriber;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("properties.json")
    .Build();

#if (!DEBUG)
try 
{
#endif 

using var loggerFactory = LoggerFactory.Create(
    builder => builder.AddConsole());


using var consumer = new InitConsumer(loggerFactory.CreateLogger<InitConsumer>());
using var config = new NEMSConfig(loggerFactory.CreateLogger<NEMSConfig>());
using var context = config.CreateContext();

consumer.Connect(configuration, context);

#if (!DEBUG)
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
#endif 
