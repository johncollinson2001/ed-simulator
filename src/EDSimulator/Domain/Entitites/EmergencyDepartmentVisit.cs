using EDSimulator.Domain.Entitites;
using EDSimulator.Domain.Enums;

namespace EDSimulator.Domain.Entities
{
    public class EmergencyDepartmentVisit
    {
        public Guid Id { get; }
        public EmergencyDepartment EmergencyDepartment { get; }
        public Patient Patient { get; }
        public DateTime StartDateTime { get; }
        public IList<EmergencyDepartmentEvent> Events { get; } = new List<EmergencyDepartmentEvent>();

        /// <summary>
        /// The latest event to have occurred in the visit.
        /// </summary>
        public EmergencyDepartmentEvent? LatestEvent => Events.OrderByDescending(e => e.StartDateTime).FirstOrDefault();

        /// <summary>
        /// States if the patient is waiting to be seen, e.g. a visit is not in progress and the patient hasn't been discharged.
        /// </summary>
        public bool IsWaitingToBeSeen => LatestEvent == null || (LatestEvent.IsCompleted && !IsDischarged);

        /// <summary>
        /// States if the patient is being seen, e.g. there's an event in progress.
        /// </summary>
        public bool IsBeingSeen => LatestEvent != null && !LatestEvent.IsCompleted;

        /// <summary>
        /// States if the patient has been discharged.
        /// </summary>
        public bool IsDischarged => LatestEvent != null && LatestEvent is EmergencyDepartmentEventDischarge && LatestEvent.IsCompleted;

        /// <summary>
        /// The priority of the visit, which is specified in the triage event.
        /// </summary>
        public CodedConcept? Priority => (Events.SingleOrDefault(e => e is EmergencyDepartmentEventTriage) as EmergencyDepartmentEventTriage)?.Coding.SingleOrDefault(c => c.CodesetType == CodesetType.Priority);

        /// <summary>
        /// Construct.
        /// </summary>
        /// <param name="emergencyDepartment">The emergency department which the patient is visiting.</param>
        /// <param name="patient">The patient visiting ED.</param>
        public EmergencyDepartmentVisit(EmergencyDepartment emergencyDepartment, Patient patient)
        {
            Id = Guid.NewGuid();
            EmergencyDepartment = emergencyDepartment ?? throw new ArgumentNullException(nameof(emergencyDepartment));
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            StartDateTime = EDSimulatorBackgroundService.SimulationDateTime;
        }

        /// <summary>
        /// Starts the next event in the visit.
        /// </summary>
        /// <param name="clinician">The clinician who's available to participate in the event</param>
        /// <returns></returns>
        public EmergencyDepartmentEvent StartNextEvent(Clinician clinician)
        {
            if (IsBeingSeen)
                throw new InvalidOperationException($"Cannot start next event for visit {Id} because the patient is currently being seen.");

            if (IsDischarged)
                throw new InvalidOperationException($"Cannot start next event for visit {Id} because the patient has been discharged.");

            switch (LatestEvent)
            {
                // Patient has arrived, requires triage
                case null:
                    StartTriageEvent(clinician);
                    break;

                // Patient has been triaged, requires assessment
                case EmergencyDepartmentEventTriage:
                    StartAssessmentEvent(clinician);
                    break;

                // Patient has been assessed, required treatment
                case EmergencyDepartmentEventAssessment:
                    StartTreatmentEvent(clinician);
                    break;

                // Patient has been treated, requires discharge
                case EmergencyDepartmentEventTreatment:
                    StartDischargeEvent(clinician);
                    break;

                default:
                    throw new ApplicationException($"Unhandled event type {LatestEvent.GetType().Name} when moving patient through ED workflow.");
            }

            // Ensure an event was started
            if (LatestEvent == null)
                throw new ApplicationException($"An event was not created when attempting to start the next event.");

            return LatestEvent;
        }

        /// <summary>
        /// Completes the latest event to be started.
        /// </summary>
        public EmergencyDepartmentEvent CompleteLatestEvent()
        {
            if (LatestEvent == null)
                throw new InvalidOperationException("No event has occurred for the visit.");

            if (LatestEvent.IsCompleted)
                throw new InvalidOperationException("The latest event has been completed.");

            LatestEvent.Complete();

            return LatestEvent;
        }

        /// <summary>
        /// Serialises the visit to a string.
        /// </summary>
        public override string ToString()
        {
            return @$"
            {{
                Visit Id: {Id}
                Patient: {{
                    Patient Id: {Patient.Id}
                    NHS Number: {Patient.NHSNumber}
                    Name: {Patient.Name.FullName}
                    Address: {Patient.Address.FullAddress}
                }}
                Priority: {Priority}
                Is Waiting to be Seen: {IsWaitingToBeSeen}
                Is Being Seen: {IsBeingSeen}
                Is Discharged: {IsDischarged}
                Events: [{string.Join(null, Events.Select(e => @$"
                    {{
                        Event Id: {e.Id}
                        Type: {e.GetType().Name}
                        Clinician: {{ 
                            Clinician Id: {e.Clinician.Id}
                            Clinician Number: {e.Clinician.ClinicianNumber}
                            Name: {e.Clinician.Name.FullName}
                        }}
                        Duration: {e.Duration}
                        Started On: {e.StartDateTime}
                        Expected Completion: {e.ExpectedCompletionDateTime}
                        Is Pending Completion: {e.IsPendingCompletion}
                        Is Completed: {e.IsCompleted}
                        Completed On: {e.CompletionDateTime}
                    }}"))}
                ]
            }}";
        }

        /// <summary>
        /// Starts the triage event.
        /// </summary>
        /// <param name="clinician">The clinician who can triage the patient.</param>
        private void StartTriageEvent(Clinician clinician)
        {
            var duration = clinician.CalculateTimeToTriage(this);

            var e = new EmergencyDepartmentEventTriage(this, clinician, duration);

            Events.Add(e);
        }

        /// <summary>
        /// Starts the assessment event.
        /// </summary>
        /// <param name="clinician">The clinician who can assess the patient.</param>
        private void StartAssessmentEvent(Clinician clinician)
        {
            var duration = clinician.CalculateTimeToAssess(this);

            var e = new EmergencyDepartmentEventAssessment(this, clinician, duration);

            Events.Add(e);
        }

        /// <summary>
        /// Starts the treatment event.
        /// </summary>
        /// <param name="clinician">The clinician who can treat the patient.</param>
        private void StartTreatmentEvent(Clinician clinician)
        {
            var duration = clinician.CalculateTimeToTreat(this);

            var e = new EmergencyDepartmentEventTreatment(this, clinician, duration);

            Events.Add(e);
        }

        /// <summary>
        /// Starts the discharge event.
        /// </summary>
        /// <param name="clinician">The clinician who can discharge the patient.</param>
        private void StartDischargeEvent(Clinician clinician)
        {
            var duration = clinician.CalculateTimeToDischarge(this);

            var e = new EmergencyDepartmentEventDischarge(this, clinician, duration);

            Events.Add(e);
        }
    }
}