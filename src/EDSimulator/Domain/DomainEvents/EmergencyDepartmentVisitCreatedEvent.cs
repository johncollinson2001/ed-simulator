using EDSimulator.Domain.Entities;
using MediatR;

namespace EDSimulator.Domain.DomainEvents
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
