using EDSimulator.Core.Enums;
using EDSimulator.Core.ValueObjects;

namespace EDSimulator.Core.Entities
{
    public class EmergencyDepartmentEventTreatment : EmergencyDepartmentEvent
    {
        public EmergencyDepartmentEventTreatment(EmergencyDepartmentVisit visit, Clinician clinician, int duration)
            : base(visit, clinician, duration)
        {
        }

        public override void Complete()
        {
            // Treat the patient
            var coding = Clinician.TreatPatient(Visit);

            Coding.AddRange(coding);

            base.Complete();
        }
    }
}