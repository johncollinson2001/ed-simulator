using EDSimulator.Core.DomainEvents;
using EDSimulator.Core.Interfaces;
using Hl7.Fhir.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EDSimulator.Infrastructure.Azure.DomainEventHandlers
{
    public class EmergencyDepartmentCreatedEventHandler : INotificationHandler<EmergencyDepartmentCreatedEvent>
    {
        private readonly IFHIRServer _fhirServer;
        private readonly ILogger<EmergencyDepartmentCreatedEventHandler> _logger;

        public EmergencyDepartmentCreatedEventHandler(IFHIRServer fhirServer, ILogger<EmergencyDepartmentCreatedEventHandler> logger)
        {
            _fhirServer = fhirServer ?? throw new ArgumentNullException(nameof(fhirServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the emergency department created event.
        /// </summary>
        /// <param name="e">Contains all the relevant details of the event.</param>
        public async System.Threading.Tasks.Task Handle(EmergencyDepartmentCreatedEvent e, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling event {e.GetType().Name}...");

            // Create organisation
            await CreateOrganisation(e);

            // Create location
            await CreateLocation(e);

            _logger.LogInformation($"{e.GetType().Name} handled successfully.");
        }

        /// <summary>
        /// Creates the organsation FHIR resource.
        /// </summary>
        private async System.Threading.Tasks.Task CreateOrganisation(EmergencyDepartmentCreatedEvent e)
        {
            _logger.LogInformation($"Creating organisation resource.");

            var organisation = new Organization()
            {
                Id = e.EmergencyDepartment.Id.ToString(),
                Identifier = new List<Identifier>()
                {
                    new Identifier()
                    {
                        Use = Identifier.IdentifierUse.Official,
                        System = "https://fhir.nhs.uk/Id/ods-organization-code",
                        Value = "RBD"
                    }
                },
                Name = "DORSET COUNTY HOSPITAL NHS FOUNDATION TRUST",
                Address = new List<Address>()
                {
                    new Address() {
                        Line = new List<string>()
                        {
                            "DORSET COUNTY HOSPITAL",
                            "WILLIAMS AVENUE"
                        },
                        City = "DORCHESTER",
                        PostalCode = "DT1 2JY",
                        Country = "ENGLAND"
                    }
                }
            };

            await _fhirServer.CreateResource(organisation);

            _logger.LogInformation($"Organisation resource created successfully.");
        }

        /// <summary>
        /// Creates the location FHIR resource.
        /// </summary>
        private async System.Threading.Tasks.Task CreateLocation(EmergencyDepartmentCreatedEvent e)
        {
            _logger.LogInformation($"Creating location resource.");

            var location = new Location()
            {
                Id = e.EmergencyDepartment.Id.ToString(),
                Identifier = new List<Identifier>()
                {
                    new Identifier()
                    {
                        System = "https://fhir.nhs.uk/Id/ods-site-code",
                        Value = "RBD01"
                    }
                },
                Name = "DORSET COUNTY HOSPITAL EMERGENCY DEPARTMENT",
                Type = new List<CodeableConcept>()
                {
                    new CodeableConcept("http://terminology.hl7.org/CodeSystem/v3-RoleCode", "ER", "Emergency Room")
                },
                Address = new Address()
                {
                    Line = new List<string>()
                    {
                        "EMERGENCY DEPARTMENT",
                        "DORSET COUNTY HOSPITAL",
                        "WILLIAMS AVENUE"
                    },
                    City = "DORCHESTER",
                    PostalCode = "DT1 2JY",
                    Country = "ENGLAND"
                }
            };

            await _fhirServer.CreateResource(location);

            _logger.LogInformation($"Location resource created successfully.");
        }
    }
}