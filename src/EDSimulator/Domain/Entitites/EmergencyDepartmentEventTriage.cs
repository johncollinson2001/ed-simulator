using EDSimulator.Domain.Enums;
using EDSimulator.Domain.ValueObjects;

namespace EDSimulator.Domain.Entities
{
    public class EmergencyDepartmentEventTriage : EmergencyDepartmentEvent
    {
        public EmergencyDepartmentEventTriage(EmergencyDepartmentVisit visit, Clinician clinician, int duration)
            : base(visit, clinician, duration)
        {
        }

        public override void Complete()
        {
            // Triage the patient
            var coding = Clinician.TriagePatient(Visit);

            Coding.AddRange(coding);

            base.Complete();
        }
    }
}