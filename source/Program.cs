using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SparkHealthSolace;

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

using var config = new SolaceConfig();
using var consumer = new QueueConsumer(loggerFactory.CreateLogger<QueueConsumer>());
consumer.Run(settings, config);

#if (!DEBUG)
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
#endif 
