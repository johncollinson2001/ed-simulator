using Azure.Core;
using Azure.Identity;
using EDSimulator.Domain.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

        public async Task<T> CreateResource<T>(T resource) where T : Resource
        {
            try
            {
                var createdResource = await _client.CreateAsync(resource);

                if (createdResource == null)
                    throw new ApplicationException($"Created resource {typeof(T).Name}/{resource.Id} was not returned from the API.");

                _logger.LogInformation($"Created FHIR resource {typeof(T).Name}/{resource.Id}");

                return createdResource;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not create FHIR resource {typeof(T).Name}/{resource.Id}: {ex.Message}");

                throw;
            }
        }

        public async Task<T> UpdateResource<T>(T resource) where T : Resource
        {
            try
            {
                var updatedResource = await _client.UpdateAsync(resource);

                if (updatedResource == null)
                    throw new ApplicationException($"Updated resource {typeof(T).Name}/{resource.Id} was not returned from the API.");

                _logger.LogInformation($"Updated FHIR resource {typeof(T).Name}/{resource.Id}");

                return updatedResource;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not create FHIR resource {typeof(T).Name}/{resource.Id}: {ex.Message}");

                throw;
            }
        }

        public async Task<T> GetResourceForId<T>(string id) where T : Resource
        {
            var uri = $"{typeof(T).Name}/{id}";

            try
            {
                var resource = await _client.ReadAsync<T>(uri);

                if (resource == null)
                    throw new ApplicationException($"Queried resource {uri} was not returned from the API.");

                _logger.LogInformation($"Queried FHIR resource {uri}");

                return resource;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not get FHIR resource {uri}: {ex.Message}");

                throw;
            }
        }
    }
}
