using EDSimulator.Core.Enums;
using EDSimulator.Core.ValueObjects;
using Hl7.Fhir.Model;

namespace EDSimulator.Core.Entities
{
    public class EmergencyDepartmentEventDischarge : EmergencyDepartmentEvent
    {
        public EmergencyDepartmentEventDischarge(EmergencyDepartmentVisit visit, Clinician clinician, int duration)
            : base(visit, clinician, duration)
        {
        }

        public override void Complete()
        {
            // Discharge the patient
            var coding = Clinician.DischargePatient(Visit);

            Coding.AddRange(coding);

            base.Complete();
        }
    }
}