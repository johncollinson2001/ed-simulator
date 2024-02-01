using EDSimulator.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EDSimulator.Infrastructure.HL7
{
    public class HL7Client : IHL7Client
    {
        private readonly HttpClient? _client;
        private readonly HL7Configuration _configuration = new();
        private readonly ILogger<HL7Client> _logger;

        public bool IsDisabled => _configuration.Disabled;

        public HL7Client(IConfiguration configuration, ILogger<HL7Client> logger)
        {
            configuration.GetSection("HL7Client").Bind(_configuration);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (_configuration.Disabled)
            {
                _logger.LogInformation($"HL7 client is disabled.");
                return;
            }

            // Create HTTP client instance
            // ...

            _client = new HttpClient();

            _logger.LogInformation($"Created HL7v2 HTTP client for endpoint {_configuration.Endpoint}.");

            if (_configuration.HttpHeaders.Any())
            {
                foreach (var header in _configuration.HttpHeaders)
                {
                    _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }

                _logger.LogInformation($"Added {_configuration.HttpHeaders.Count} HTTP headers.");
            }

            // TODO: Auth here?
        }

        public async Task SendMessage(string message)
        {
            if (_configuration.Disabled)
            {
                _logger.LogInformation($"HL7 client is disabled.");
                return;
            }

            try
            {
                message = message.Replace("{SENDING_ORGANISATION}", _configuration.SendingOrganisation);
                message = message.Replace("{SENDING_APPLICATION}", _configuration.SendingApplication);
                message = message.Replace("{RECEIVING_ORGANISATION}", _configuration.ReceivingOrganisation);
                message = message.Replace("{RECEIVING_APPLICATION}", _configuration.ReceivingApplication);

                var content = new StringContent(message);

                await _client!.PostAsync(_configuration.Endpoint, content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not send HL7v2 message: {ex.Message}");

                throw;
            }
        }
    }
}
