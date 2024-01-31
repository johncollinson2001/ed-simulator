using EDSimulator.Core.Enums;
using EDSimulator.Core.ValueObjects;

namespace EDSimulator.Core.Entities
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