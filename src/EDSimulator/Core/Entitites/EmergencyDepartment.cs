namespace EDSimulator.Core.Entities
{
    public class EmergencyDepartment
    {
        public Guid Id { get; }
        public IList<Clinician> Clinicians { get; } = new List<Clinician>();
        public IList<EmergencyDepartmentVisit> Visits { get; } = new List<EmergencyDepartmentVisit>();

        /// <summary>
        /// Construct.
        /// </summary>
        public EmergencyDepartment()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Registers a patient who has arrived at the department.
        /// </summary>
        /// <param name="patient">The patient being registered</param>
        public EmergencyDepartmentVisit CreateVisit(Patient patient)
        {
            var visit = new EmergencyDepartmentVisit(this, patient);

            Visits.Add(visit);

            return visit;
        }

        /// <summary>
        /// Adds a clinician to the department.
        /// </summary>
        /// <param name="clinician">The clinician being added</param>
        public void AddClinician(Clinician clinician)
        {
            Clinicians.Add(clinician);
        }

        /// <summary>
        /// Gets an available clinician, or null if not clinician is available.
        /// </summary>
        public Clinician? GetAvailableClinician()
        {
            var unavailableClinicianIds = Visits.Where(v => v.IsBeingSeen).Select(v => v.LatestEvent?.Clinician.Id);

            // Get an available clinician.
            // Order the clinicians randomly to ensure different clinicians are used - to spread the load across the team 
            var clinician = Clinicians
                .Where(c => !unavailableClinicianIds.Contains(c.Id))
                .OrderBy(c => Guid.NewGuid())
                .FirstOrDefault();

            return clinician;
        }
    }
}