using EDSimulator.Core.Entities;
using MediatR;

namespace EDSimulator.Core.DomainEvents
{
    public class EmergencyDepartmentVisitCreatedEvent : INotification
    {
        public EmergencyDepartmentVisit Visit { get; set; }
     
        public EmergencyDepartmentVisitCreatedEvent(EmergencyDepartmentVisit visit)
        {
            Visit = visit ?? throw new ArgumentNullException(nameof(visit));
        }
    }
}
