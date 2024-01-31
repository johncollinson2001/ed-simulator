using EDSimulator.Core.Enums;
using EDSimulator.Core.ValueObjects;

namespace EDSimulator.Core.Entities
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