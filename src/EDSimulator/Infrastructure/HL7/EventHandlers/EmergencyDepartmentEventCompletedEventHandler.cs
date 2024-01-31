using EDSimulator.Core.DomainEvents;
using EDSimulator.Core.Entities;
using EDSimulator.Core.Enums;
using EDSimulator.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EDSimulator.Infrastructure.HL7.DomainEventHandlers
{
    public class EmergencyDepartmentEventCompletedEventHandler : INotificationHandler<EmergencyDepartmentEventCompletedEvent>
    {
        private readonly IHL7Client _hl7Client;
        private readonly ILogger<EmergencyDepartmentVisitCreatedEventHandler> _logger;

        public EmergencyDepartmentEventCompletedEventHandler(IHL7Client hl7Client, ILogger<EmergencyDepartmentVisitCreatedEventHandler> logger)
        {
            _hl7Client = hl7Client ?? throw new ArgumentNullException(nameof(hl7Client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the emergency department completed event.
        /// </summary>
        /// <param name="e">Contains all the relevant details of the event.</param>
        public async Task Handle(EmergencyDepartmentEventCompletedEvent e, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling event {e.GetType().Name}...");

            switch (e.Event)
            {
                case EmergencyDepartmentEventDischarge:
                    await SendA03Message(e);
                    break;

                default:
                    _logger.LogInformation($"HL7 message handler not implemented for event type {e.GetType().Name}.");
                    break;
            }

            _logger.LogInformation($"{e.GetType().Name} handled successfully.");
        }

        /// <summary>
        /// Sends an A03 HL7 message.
        /// </summary>
        private async Task SendA03Message(EmergencyDepartmentEventCompletedEvent e)
        {
            _logger.LogInformation($"Sending HL7 A03 message.");

            var visit = e.Event.Visit;
            var patient = visit.Patient;

            var triageEvent = visit.Events.Single(e => e is EmergencyDepartmentEventTriage);
            var assessmentEvent = visit.Events.Single(e => e is EmergencyDepartmentEventAssessment);
            var treatmentEvent = visit.Events.Single(e => e is EmergencyDepartmentEventTreatment);
            var dischargeEvent = e.Event;

            var chiefComplaint = triageEvent.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareChiefComplaint);
            var priority = triageEvent.Coding.Single(c => c.CodesetType == CodesetType.Priority);
            var diagnosis = assessmentEvent.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareDiagnosis);
            var treatment = treatmentEvent.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareTreatment);
            var dischargeMethod = dischargeEvent.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareDischargeMethod);
            var dischargeStatus = dischargeEvent.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareDischargeStatus);

            var message = new StringBuilder();
            message.Append($"MSH|^~\\&|{{SENDING_APPLICATION}}|{{SENDING_ORGANISATION}}|{{RECEIVING_APPLICATION}}|{{RECEIVING_ORGANISATION}}|{DateTime.Now.ToString("yyyymmddHHmmss")}||ADT^A03|{Guid.NewGuid()}|P|2.4|||AL|NE");
            message.Append(Environment.NewLine);
            message.Append($"PID|1|{patient.Id}|{patient.NHSNumber}^^^NHS^NHSNumber||{patient.Name.Surname}^{patient.Name.FirstName}||{patient.DateOfBirth.ToString("yyyymmdd")}||||{patient.Address.Street}^^{patient.Address.City}^{patient.Address.County}^{patient.Address.Postcode}^^H|||||||||||A");
            message.Append(Environment.NewLine);
            message.Append($"PV1|1|E||F|||||||||||||||{visit.Id}|||||||||||||||||{dischargeStatus}||||||^^^{{SENDING_ORGANISATION}}||{visit.StartDateTime.ToString("yyyymmddHHmmss")}|{dischargeEvent.CompletionDateTime!.Value.ToString("yyyymmddHHmmss")}");
            message.Append(Environment.NewLine);
            message.Append($"DG1|1||{diagnosis.Code}");
            message.Append(Environment.NewLine);
            message.Append($"DG2|2||{chiefComplaint.Code}");
            message.Append(Environment.NewLine);
            message.Append($"PR1|2||{treatment.Code}");
            message.Append(Environment.NewLine);

            await _hl7Client.SendMessage(message.ToString());

            _logger.LogInformation($"HL7 A03 message sent successfully.");
        }
    }
}