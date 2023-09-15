using EDSimulator.Domain.Enums;
using EDSimulator.Domain.ValueObjects;

namespace EDSimulator.Domain.Entities
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