using EDSimulator.Domain.DomainEvents;
using EDSimulator.Domain.Interfaces;
using Hl7.Fhir.Model;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EDSimulator.Infrastructure.Azure.DomainEventHandlers
{
    public class ClinicianAddedEventHandler : INotificationHandler<ClinicianAddedEvent>
    {
        private readonly IFHIRServer _fhirServer;
        private readonly ILogger<ClinicianAddedEventHandler> _logger;

        public ClinicianAddedEventHandler(IFHIRServer fhirServer, ILogger<ClinicianAddedEventHandler> logger)
        {
            _fhirServer = fhirServer ?? throw new ArgumentNullException(nameof(fhirServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the emergency department created event.
        /// </summary>
        /// <param name="e">Contains all the relevant details of the event.</param>
        public async System.Threading.Tasks.Task Handle(ClinicianAddedEvent e, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling event {e.GetType().Name}...");

            // Create practitioner
            await CreatePractitioner(e);

            _logger.LogInformation($"{e.GetType().Name} handled successfully.");
        }

        /// <summary>
        /// Creates the practitioner FHIR resource.
        /// </summary>
        private async System.Threading.Tasks.Task CreatePractitioner(ClinicianAddedEvent e)
        {
            _logger.LogInformation($"Creating practitioner resource.");

            var Practitioner = new Practitioner()
            {
                Id = e.Clinician.Id.ToString(),
                Identifier = new List<Identifier>()
                {
                    new Identifier()
                    {
                        System = "https://fhir.nhs.uk/Id/sds-user-id",
                        Value = e.Clinician.ClinicianNumber
                    }
                },
                Name = new List<HumanName>()
                {
                    new HumanName()
                    {
                        Family = e.Clinician.Name.Surname,
                        Given = new List<string>() {
                            e.Clinician.Name.FirstName
                        }
                    }
                }
            };

            await _fhirServer.CreateResource(Practitioner);

            _logger.LogInformation($"Practitioner resource created successfully.");
        }
    }
}