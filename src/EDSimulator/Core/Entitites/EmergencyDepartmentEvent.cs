using EDSimulator.Core.Entitites;
using EDSimulator.Core.Enums;

namespace EDSimulator.Core.Entities
{
    public abstract class EmergencyDepartmentEvent
    {
        public Guid Id { get; set; }
        public EmergencyDepartmentVisit Visit { get; }
        public Clinician Clinician { get; }
        public int Duration { get; }
        public DateTime StartDateTime { get; }
        public DateTime? CompletionDateTime { get; private set; }
        public List<CodedConcept> Coding { get; } = new List<CodedConcept>();

        /// <summary>
        /// The date/time that the event is expected to complete, based on the specified duration of 
        /// the event.
        /// </summary>
        public DateTime ExpectedCompletionDateTime => StartDateTime.AddMinutes(Duration);

        /// <summary>
        /// States if the event is pending completion, e.g. the expected competion date/time has passed and
        /// the event has not been completed.
        /// </summary>
        public bool IsPendingCompletion => EDSimulatorBackgroundService.SimulationDateTime > ExpectedCompletionDateTime && !IsCompleted;

        /// <summary>
        /// States if the event has been completed.
        /// </summary>
        public bool IsCompleted => CompletionDateTime.HasValue;

        /// <summary>
        /// Construct.
        /// </summary>
        /// <param name="visit">The visit which the event is part of.</param>
        /// <param name="clinician">The clinician performing the event.</param>
        /// <param name="duration">The duration of the event.</param>
        public EmergencyDepartmentEvent(EmergencyDepartmentVisit visit, Clinician clinician, int duration)
        {
            Id = Guid.NewGuid();
            Visit = visit ?? throw new ArgumentNullException(nameof(visit));
            Clinician = clinician ?? throw new ArgumentNullException(nameof(clinician));
            Duration = duration;
            StartDateTime = EDSimulatorBackgroundService.SimulationDateTime;
        }

        /// <summary>
        /// Completes the event.
        /// </summary>
        public virtual void Complete()
        {
            CompletionDateTime = EDSimulatorBackgroundService.SimulationDateTime;
        }
    }
}