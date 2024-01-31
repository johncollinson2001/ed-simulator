using EDSimulator.Core.Entities;
using MediatR;

namespace EDSimulator.Core.DomainEvents
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
