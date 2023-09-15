using EDSimulator.Domain.Entities;
using MediatR;

namespace EDSimulator.Domain.DomainEvents
{
    public class EmergencyDepartmentCreatedEvent : INotification
    {
        public EmergencyDepartment EmergencyDepartment { get; set; }
     
        public EmergencyDepartmentCreatedEvent(EmergencyDepartment emergencyDepartment)
        {
            EmergencyDepartment = emergencyDepartment ?? throw new ArgumentNullException(nameof(emergencyDepartment));
        }
    }
}
