using EDSimulator.Core.Entities;
using MediatR;

namespace EDSimulator.Core.DomainEvents
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
