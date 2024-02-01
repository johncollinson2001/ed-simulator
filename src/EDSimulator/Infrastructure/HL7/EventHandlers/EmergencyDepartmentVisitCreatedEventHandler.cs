using EDSimulator.Core.DomainEvents;
using EDSimulator.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EDSimulator.Infrastructure.HL7.DomainEventHandlers
{
    public class EmergencyDepartmentVisitCreatedEventHandler : INotificationHandler<EmergencyDepartmentVisitCreatedEvent>
    {
        private readonly IHL7Client _hl7Client;
        private readonly ILogger<EmergencyDepartmentVisitCreatedEventHandler> _logger;

        public EmergencyDepartmentVisitCreatedEventHandler(IHL7Client hl7Client, ILogger<EmergencyDepartmentVisitCreatedEventHandler> logger)
        {
            _hl7Client = hl7Client ?? throw new ArgumentNullException(nameof(hl7Client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the emergency department created event.
        /// </summary>
        /// <param name="e">Contains all the relevant details of the event.</param>
        public async Task Handle(EmergencyDepartmentVisitCreatedEvent e, CancellationToken cancellationToken)
        {
            if (_hl7Client.IsDisabled)
                return;

            await SendA01Message(e);
        }

        /// <summary>
        /// Sends an A01 HL7 message.
        /// </summary>
        private async Task SendA01Message(EmergencyDepartmentVisitCreatedEvent e)
        {
            _logger.LogInformation($"Sending HL7 A01 message for visit {e.Visit.Id}.");

            var message = new StringBuilder();
            message.Append($"MSH|^~\\&|{{SENDING_APPLICATION}}|{{SENDING_ORGANISATION}}|{{RECEIVING_APPLICATION}}|{{RECEIVING_ORGANISATION}}|{DateTime.Now.ToString("yyyyMMddHHmmss")}||ADT^A01|{Guid.NewGuid()}|P|2.4|||AL|NE");
            message.Append(Environment.NewLine);
            message.Append($"PID|1|{e.Visit.Patient.Id}|{e.Visit.Patient.NHSNumber}^^^NHS^NHSNumber||{e.Visit.Patient.Name.Surname}^{e.Visit.Patient.Name.FirstName}||{e.Visit.Patient.DateOfBirth.ToString("yyyyMMdd")}||||{e.Visit.Patient.Address.Street}^^{e.Visit.Patient.Address.City}^{e.Visit.Patient.Address.County}^{e.Visit.Patient.Address.Postcode}^^H|||||||||||A");
            message.Append(Environment.NewLine);
            message.Append($"PV1|1|E||F|||||||||||||||{e.Visit.Id}|||||||||||||||||||||||^^^{{SENDING_ORGANISATION}}||{e.Visit.StartDateTime.ToString("yyyyMMddHHmmss")}");
            message.Append(Environment.NewLine);

            await _hl7Client.SendMessage(message.ToString());

            _logger.LogInformation($"HL7 A01 message sent successfully for visit {e.Visit.Id}.");
        }
    }
}