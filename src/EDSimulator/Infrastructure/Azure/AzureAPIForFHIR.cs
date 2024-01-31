using Azure.Core;
using Azure.Identity;
using EDSimulator.Core.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace EDSimulator.Infrastructure.Azure
{
    public class AzureAPIForFHIR : IFHIRServer
    {
        private readonly FhirClient _client;
        private readonly FHIRServerConfiguration _configuration = new();
        private readonly ILogger<AzureAPIForFHIR> _logger;

        public AzureAPIForFHIR(IConfiguration configuration, ILogger<AzureAPIForFHIR> logger)
        {
            configuration.GetSection("FHIRServer").Bind(_configuration);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create FHIR client instance
            // ...

            var settings = new FhirClientSettings
            {
                PreferredFormat = ResourceFormat.Json,
                VerifyFhirVersion = true,
                ReturnPreference = ReturnPreference.Minimal
            };

            _client = new FhirClient(_configuration.Endpoint, settings);

            _logger.LogInformation($"Created FHIR client for endpoint {_configuration.Endpoint}.");

            // Fetch auth token from Azure and add the client HTTP auth header
            // NOTE - DefaultAzureCredential relies on environment variables for client id/client secret/tenant id
            // which is set in launchSettings in dev, and via other methods in other environments
            // ...

            _logger.LogInformation("Logging into Azure...");

            var credential = new DefaultAzureCredential();

            var token = credential.GetToken(new TokenRequestContext(new[] { $"{_configuration.Endpoint}/.default" }));

            _client.RequestHeaders?.Add("Authorization", $"Bearer {token.Token}");

            _logger.LogInformation($"Authorisation token received and added to FHIR client Authorization request header.");
        }

        public async Task CreateResource<T>(T resource) where T : Resource
        {
            if (!_configuration.Enabled)
            {
                _logger.LogInformation($"FHIR server is disabled.");
                return;
            }

            try
            {
                var createdResource = await _client.CreateAsync(resource);

                if (createdResource == null)
                    throw new ApplicationException($"Created resource {typeof(T).Name}/{resource.Id} was not returned from the API.");

                _logger.LogInformation($"Created FHIR resource {typeof(T).Name}/{resource.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not create FHIR resource {typeof(T).Name}/{resource.Id}: {ex.Message}");

                throw;
            }
        }

        public async Task UpdateResource<T>(T resource) where T : Resource
        {
            if (!_configuration.Enabled)
            {
                _logger.LogInformation($"FHIR server is disabled.");
                return;
            }

            try
            {
                var updatedResource = await _client.UpdateAsync(resource);

                if (updatedResource == null)
                    throw new ApplicationException($"Updated resource {typeof(T).Name}/{resource.Id} was not returned from the API.");

                _logger.LogInformation($"Updated FHIR resource {typeof(T).Name}/{resource.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not create FHIR resource {typeof(T).Name}/{resource.Id}: {ex.Message}");

                throw;
            }
        }
    }
}
