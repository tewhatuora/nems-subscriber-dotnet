using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GuaranteedSubscriber;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("properties.json")
    .Build();

var queueSection = configuration.GetSection("QueueConsumer");

var settings = new QueueConsumerSettings
{
    Host = queueSection["Host"] ?? string.Empty,
    VPNName = queueSection["VPNName"] ?? string.Empty,
    Username = queueSection["Username"] ?? string.Empty,
    Password = queueSection["Password"] ?? string.Empty,
    QueueName = queueSection["QueueName"] ?? string.Empty,
};

#if (!DEBUG)
try 
{
#endif 

using var loggerFactory = LoggerFactory.Create(
    builder => builder.AddConsole());


using var consumer = new QueueConsumer(loggerFactory.CreateLogger<QueueConsumer>());

using var config = new NEMSConfig(loggerFactory.CreateLogger<NEMSConfig>());
using var context = config.CreateContext();
consumer.Run(settings, context);

#if (!DEBUG)
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
#endif 
