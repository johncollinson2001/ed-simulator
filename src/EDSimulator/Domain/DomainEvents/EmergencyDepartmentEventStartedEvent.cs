using EDSimulator.Domain.Entities;
using MediatR;

namespace EDSimulator.Domain.DomainEvents
{
    public class EmergencyDepartmentEventStartedEvent : INotification
    {
        public EmergencyDepartmentEvent Event { get; set; }
     
        public EmergencyDepartmentEventStartedEvent(EmergencyDepartmentEvent e)
        {
            Event = e ?? throw new ArgumentNullException(nameof(e));
        }
    }
}
