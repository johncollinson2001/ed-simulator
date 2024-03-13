using EDSimulator.Core.Entitites;
using EDSimulator.Core.Enums;
using EDSimulator.Core.ValueObjects;

namespace EDSimulator.Core.Entities
{
    public class Clinician
    {
        public Guid Id { get; }
        public string ClinicianNumber { get; }
        public Name Name { get; }

        /// <summary>
        /// Construct.
        /// </summary>
        /// <param name="clinicianNumber">The clinician's professional number.</param>
        /// <param name="name">The clinician's name.</param>
        public Clinician(string clinicianNumber, Name name)
        {
            Id = Guid.NewGuid();
            ClinicianNumber = clinicianNumber ?? throw new ArgumentNullException(nameof(clinicianNumber));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Calculates the time in minutes for the clinician to discharge the patient.
        /// </summary>
        /// <param name="visit">The visit being discharged.</param>
        public int CalculateTimeToDischarge(EmergencyDepartmentVisit visit)
        {
            // NOTE: This formula should clearly include the diagnosis and treatment, however
            // this is yet to be implemented, and needs so clinical knowlegde to grade the 
            // coding so they can be weighted here.

            var min = 2
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 4 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 2 : 0)
                + (visit.Priority?.Code == "EM" ? 4 : visit.Priority?.Code == "UR" ? 2 : 0);

            var max = 5
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 20 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 5 : 0)
                + (visit.Priority?.Code == "EM" ? 20 : visit.Priority?.Code == "UR" ? 10 : 0);

            return new Random().Next(min, max);
        }

        /// <summary>
        /// Calculates the time in minutes for the treat to discharge the patient.
        /// </summary>
        /// <param name="visit">The visit being treated.</param>
        public int CalculateTimeToTreat(EmergencyDepartmentVisit visit)
        {
            // NOTE: This formula should clearly include the diagnosis, however this is
            // yet to be implemented, and needs so clinical knowlegde to grade the coding
            // so they can be weighted here.

            var min = 1
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 4 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 2 : 0)
                + (visit.Priority?.Code == "EM" ? 4 : visit.Priority?.Code == "UR" ? 2 : 0);

            var max = 5
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 60 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 10 : 0)
                + (visit.Priority?.Code == "EM" ? 60 : visit.Priority?.Code == "UR" ? 20 : 0);

            return new Random().Next(min, max);
        }

        /// <summary>
        /// Calculates the time in minutes for the clinician to assess the patient.
        /// </summary>
        /// <param name="patient">The patient being assessed - currently un-used, but left in for future use.</param>
        public int CalculateTimeToAssess(EmergencyDepartmentVisit visit)
        {
            // NOTE: This formula should clearly include the chief complaint, however this is
            // yet to be implemented, and needs so clinical knowlegde to grade the coding
            // so they can be weighted here.

            var min = 1
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 4 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 2 : 0)
                + (visit.Priority?.Code == "EM" ? 4 : visit.Priority?.Code == "UR" ? 2 : 0);

            var max = 2
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 20 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 4 : 0)
                + (visit.Priority?.Code == "EM" ? 20 : visit.Priority?.Code == "UR" ? 4 : 0);

            return new Random().Next(min, max);
        }

        /// <summary>
        /// Calculates the time in minutes for the clinician to triage the patient.
        /// </summary>
        /// <param name="patient">The visit being triaged.</param>
        public int CalculateTimeToTriage(EmergencyDepartmentVisit visit)
        {
            var min = 1
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 2 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 1 : 0);

            var max = 2
                + (visit.Patient.Age < 3 || visit.Patient.Age > 80 ? 5 : visit.Patient.Age < 18 || visit.Patient.Age > 65 ? 2 : 0);

            return new Random().Next(min, max);
        }

        /// <summary>
        /// Triages a patient.
        /// </summary>
        /// <param name="visit">The visit to triage - currently un-used, but left in for future use.</param>
        public IEnumerable<CodedConcept> TriagePatient(EmergencyDepartmentVisit visit)
        {
            var chiefComplaint = Codesets.EmergencyCareChiefComplaint.GetRandomEntry();

            var priority = Codesets.Priority.GetRandomEntry();

            var coding = new List<CodedConcept>()
            {
                new CodedConcept(CodesetType.EmergencyCareChiefComplaint, chiefComplaint.Key, chiefComplaint.Value),
                new CodedConcept(CodesetType.Priority, priority.Key, priority.Value)
            };

            return coding;
        }

        /// <summary>
        /// Diagnoses a patient.
        /// </summary>
        /// <param name="visit">The visit to diagnose - currently un-used, but left in for future use.</param>
        public IEnumerable<CodedConcept> DiagnosePatient(EmergencyDepartmentVisit visit)
        {
            var diagnosis = Codesets.EmergencyCareDiagnosis.GetRandomEntry();

            var coding = new List<CodedConcept>()
            {
                new CodedConcept(CodesetType.EmergencyCareDiagnosis, diagnosis.Key, diagnosis.Value)
            };

            return coding;
        }

        /// <summary>
        /// Treats a patient.
        /// </summary>
        /// <param name="visit">The visit to treat - currently un-used, but left in for future use.</param>
        public IEnumerable<CodedConcept> TreatPatient(EmergencyDepartmentVisit visit)
        {
            var treatment = Codesets.EmergencyCareTreatment.GetRandomEntry();

            var coding = new List<CodedConcept>()
            {
                new CodedConcept(CodesetType.EmergencyCareTreatment, treatment.Key, treatment.Value)
            };

            return coding;
        }

        /// <summary>
        /// Discharges a patient.
        /// </summary>
        /// <param name="visit">The visit to discharge - currently un-used, but left in for future use.</param>
        public IEnumerable<CodedConcept> DischargePatient(EmergencyDepartmentVisit visit)
        {
            var dischargeMethod = Codesets.EmergencyCareDischargeMethod.GetRandomEntry();
            var dischargeStatus = Codesets.EmergencyCareDischargeStatus.GetRandomEntry();

            var coding = new List<CodedConcept>() 
            {
                new CodedConcept(CodesetType.EmergencyCareDischargeMethod, dischargeMethod.Key, dischargeMethod.Value),
                new CodedConcept(CodesetType.EmergencyCareDischargeStatus, dischargeStatus.Key, dischargeStatus.Value)
            };

            return coding;
        }
    }
}