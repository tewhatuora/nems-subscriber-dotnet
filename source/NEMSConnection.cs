using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SolaceSystems.Solclient.Messaging;
using Newtonsoft.Json;

namespace GuaranteedSubscriber;
public class NEMSConnection
{
    private readonly ILogger<NEMSConnection> _logger;

    public NEMSConnection()
    {
        _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole(); // Add other logging providers as needed
        }).CreateLogger<NEMSConnection>();
    }

    public SessionProperties OAuthConnection(IConfiguration configuration)
    {

        string token = RenewToken(configuration);
        var nemsProperties = configuration.GetSection("NEMSProperties");
        var oauthProperties = configuration.GetSection("OAuthCredentials");
        var oauthSettings = new OAuthSettings
        {
            Host = nemsProperties["Host"] ?? string.Empty,
            VPNName = nemsProperties["VPNName"] ?? string.Empty,
            Token = token,
            Issuer = oauthProperties["Issuer"] ?? string.Empty,
            DefaultReconnectRetries = nemsProperties.GetValue<int>("ReconnectRetries")
        };

        var connectionProperties = oauthSettings.setProperties();

        return connectionProperties;
    }

    public SessionProperties BasicConnection(IConfiguration configuration)
    {

        var basicProperties = configuration.GetSection("BasicCredentials");
        var nemsProperties = configuration.GetSection("NEMSProperties");
        var basicSettings = new BasicSettings
        {
            Host = nemsProperties["Host"] ?? string.Empty,
            VPNName = nemsProperties["VPNName"] ?? string.Empty,
            UserName = basicProperties["Username"] ?? string.Empty,
            Password = basicProperties["Password"] ?? string.Empty,
            DefaultReconnectRetries = nemsProperties.GetValue<int>("ReconnectRetries")
        };

        var connectionProperties = basicSettings.setProperties();

        return connectionProperties;
    }
    public string RenewToken(IConfiguration configuration)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        var oauthProperties = configuration.GetSection("OAuthCredentials");
        var tokenServer = oauthProperties["TokenServer"];
        var clientId = oauthProperties["ClientId"];
        var clientSecret = oauthProperties["ClientSecret"];
        var scope = oauthProperties["Scope"];

        using (var client = new HttpClient())
        {

            var request = new HttpRequestMessage(HttpMethod.Post, tokenServer);
            var requestBody = $"client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials&scope={scope}";
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = client.Send(request);

            if (response.IsSuccessStatusCode)
            {

                var responsePayload = response.Content.ReadAsStringAsync().Result;
                var token = JsonConvert.DeserializeObject<Token>(responsePayload);
                if (token != null)
                {
                    _logger.LogInformation("Token created successfully");
                    return token.AccessToken;
                }
                else
                {
                    throw new Exception($"Failed to retrieve access token: {response.ReasonPhrase}");
                }
            }
            else
            {
                throw new Exception($"Failed to retrieve access token: {response.ReasonPhrase}");
            }
        }

    }

    internal class Token
    {
        [JsonProperty("access_token")]
        public required string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public required string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public required int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public required string RefreshToken { get; set; }
    }
}