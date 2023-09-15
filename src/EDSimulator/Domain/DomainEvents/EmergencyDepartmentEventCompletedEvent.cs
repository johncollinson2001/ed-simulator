using EDSimulator.Domain.Entities;
using MediatR;

namespace EDSimulator.Domain.DomainEvents
{
    public class EmergencyDepartmentEventCompletedEvent : INotification
    {
        public EmergencyDepartmentEvent Event { get; set; }
     
        public EmergencyDepartmentEventCompletedEvent(EmergencyDepartmentEvent e)
        {
            Event = e ?? throw new ArgumentNullException(nameof(e));
        }
    }
}
