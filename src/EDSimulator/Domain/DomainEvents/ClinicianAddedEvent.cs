using EDSimulator.Domain.Entities;
using MediatR;

namespace EDSimulator.Domain.DomainEvents
{
    public class ClinicianAddedEvent : INotification
    {
        public Clinician Clinician { get; set; }
     
        public ClinicianAddedEvent(Clinician clinician)
        {
            Clinician = clinician ?? throw new ArgumentNullException(nameof(clinician));
        }
    }
}
