using EDSimulator.Domain.Enums;
using EDSimulator.Domain.ValueObjects;

namespace EDSimulator.Domain.Entities
{
    public class EmergencyDepartmentEventAssessment : EmergencyDepartmentEvent
    {
        public EmergencyDepartmentEventAssessment(EmergencyDepartmentVisit visit, Clinician clinician, int duration)
            : base(visit, clinician, duration)
        {
        }

        public override void Complete()
        {
            // Assess the patient
            var coding = Clinician.DiagnosePatient(Visit);

            Coding.AddRange(coding);

            base.Complete();
        }
    }
}