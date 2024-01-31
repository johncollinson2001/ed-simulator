using EDSimulator.Core.Entities;
using MediatR;

namespace EDSimulator.Core.DomainEvents
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
