using EDSimulator.Core.DomainEvents;
using EDSimulator.Core.Entities;
using EDSimulator.Core.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EDSimulator.Infrastructure.Azure.DomainEventHandlers
{
    public class EmergencyDepartmentEventStartedEventHandler : INotificationHandler<EmergencyDepartmentEventStartedEvent>
    {
        private readonly IFHIRServer _fhirServer;
        private readonly ILogger<EmergencyDepartmentEventStartedEventHandler> _logger;

        public EmergencyDepartmentEventStartedEventHandler(IFHIRServer fhirServer, ILogger<EmergencyDepartmentEventStartedEventHandler> logger)
        {
            _fhirServer = fhirServer ?? throw new ArgumentNullException(nameof(fhirServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the emergency department created event.
        /// </summary>
        /// <param name="e">Contains all the relevant details of the event.</param>
        public async System.Threading.Tasks.Task Handle(EmergencyDepartmentEventStartedEvent e, CancellationToken cancellationToken)
        {
            if (_fhirServer.IsDisabled)
                return;

            _logger.LogInformation($"Handling event {e.GetType().Name}...");

            await UpdateEncounter(e);

            _logger.LogInformation($"{e.GetType().Name} handled successfully.");
        }

        /// <summary>
        /// Updates the encounter FHIR resource.
        /// </summary>
        private async System.Threading.Tasks.Task UpdateEncounter(EmergencyDepartmentEventStartedEvent e)
        {
            _logger.LogInformation($"Updating encounter resource for visit {e.Event.Visit.Id}.");

            var participantType = e.Event is EmergencyDepartmentEventDischarge
                ? new CodeableConcept("http://terminology.hl7.org/CodeSystem/v3-ParticipationType", "DIS", "discharger")
                : new CodeableConcept("http://terminology.hl7.org/CodeSystem/v3-ParticipationType", "ATND", "attender");

            var encounter = new Encounter()
            {
                Id = e.Event.Visit.Id.ToString(),
                Status = Encounter.EncounterStatus.InProgress,
                Participant = new List<Encounter.ParticipantComponent>()
                {
                    new Encounter.ParticipantComponent()
                    {
                        Type = new List<CodeableConcept>()
                        {
                            participantType
                        },
                        Individual = new ResourceReference($"Practitioner/{e.Event.Clinician.Id}"),
                        Period = new Period()
                        {
                            Start = e.Event.StartDateTime.ToFhirDateTime(TimeZoneInfo.Utc.BaseUtcOffset)
                        },
                    }
                }
            };

            await _fhirServer.UpdateResource(encounter);

            _logger.LogInformation($"Encounter resource updated successfully for visit {e.Event.Visit.Id}.");
        }
    }
}