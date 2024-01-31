using EDSimulator.Core.Entities;
using MediatR;

namespace EDSimulator.Core.DomainEvents
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
