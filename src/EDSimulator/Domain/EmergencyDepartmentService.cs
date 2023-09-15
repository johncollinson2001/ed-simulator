using EDSimulator.Domain.Entities;
using EDSimulator.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using EDSimulator.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace EDSimulator.Domain
{
    public interface IEmergencyDepartmentService
    {
        void AddClinician();
        void CreateVisit();
        void UpdateState();
    }

    public class EmergencyDepartmentVisitComparer : IComparer<EmergencyDepartmentVisit>
    {
        public int Compare(EmergencyDepartmentVisit? a, EmergencyDepartmentVisit? b)
        {
            if (a?.Priority?.Code == "EM")
                return 1;
            else if (b?.Priority?.Code == "EM")
                return -1;
            else if (a?.Priority?.Code == "UR")
                return 1;
            else if (b?.Priority?.Code == "UR")
                return -1;
            else if (a?.Priority is null)
                return 1;
            else if (b?.Priority is null)
                return -1;
            else
                return 1;
        }
    }

    public sealed class EmergencyDepartmentService : IEmergencyDepartmentService
    {
        private readonly EmergencyDepartment _emergencyDepartment;
        private readonly IMediator _mediator;
        private readonly ILogger<EmergencyDepartmentService> _logger;

        /// <summary>
        /// App config
        /// </summary>
        public static EDSimulatorConfiguration Configuration { get; } = new();

        /// <summary>
        /// Construct with an ED to work with.
        /// </summary>
        /// <param name="emergencyDepartment"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EmergencyDepartmentService(IMediator mediator, ILogger<EmergencyDepartmentService> logger, IConfiguration configuration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            configuration.Bind(Configuration);

            _emergencyDepartment = new EmergencyDepartment();

            _logger.LogInformation("Emergency department created.");

            _mediator.Publish(new EmergencyDepartmentCreatedEvent(_emergencyDepartment));
        }

        /// <summary>
        /// Adds a clinician to the ED.
        /// </summary>
        public void AddClinician()
        {
            var clinician = GenerateClinician();

            _emergencyDepartment.AddClinician(clinician);

            _logger.LogInformation($"Added clinician {clinician.ClinicianNumber} / {clinician.Name.FullName}.");

            _mediator.Publish(new ClinicianAddedEvent(clinician));
        }

        /// <summary>
        /// Creates a visit to the ED.
        /// </summary>
        public void CreateVisit()
        {
            // Figure out if the patient is a new patient to the department, or a returning
            // patient. To do this, calculate a random between 0 and the size of the population
            // and see if the number of discharged patients is below this value. If it is then
            // randomly select a discharged patient.

            var dischargedPatients = _emergencyDepartment.Visits
                .Where(v => v.IsDischarged)
                .Select(v => v.Patient)
                .GroupBy(p => p.Id)
                .Select(g => g.First());

            var patient = dischargedPatients.Count() >= new Random().Next(0, Configuration.SizeOfPopulation + 1)
                ? dischargedPatients.OrderBy(p => Guid.NewGuid()).First()
                : GeneratePatient();

            var visit = _emergencyDepartment.CreateVisit(patient);

            var numberOfPreviousVisits = dischargedPatients.Count(p => p.Id == patient.Id);

            _logger.LogInformation($"Created visit for patient {visit.Patient.Id} / {visit.Patient.NHSNumber} / {visit.Patient.Name.FullName} ({numberOfPreviousVisits} previous visit{(numberOfPreviousVisits == 1 ? string.Empty: "s")}).");

            _mediator.Publish(new EmergencyDepartmentVisitCreatedEvent(visit));
        }

        /// <summary>
        /// Updates the state of the department, by iterating through each patient
        /// and moving them forward in their workflow.
        /// </summary>
        public void UpdateState()
        {
            _logger.LogInformation($"Updating state. The simulation date/time is {EDSimulatorBackgroundService.SimulationDateTime}.");

            // Process visits which are current being seen
            ProcessVisitsBeingSeen();

            // Process waiting visits in the department
            ProcessVisitsWaitingToBeSeen();
        }

        /// <summary>
        /// Processes the visist currently being seen in the ED.
        /// </summary>
        private void ProcessVisitsBeingSeen()
        {
            _logger.LogInformation($"{_emergencyDepartment.Visits.Count(v => v.IsBeingSeen)} visits currently being seen.");

            // Find visits where the last event is pending completion
            var visitsPendingCompletion = _emergencyDepartment.Visits.Where(v => v.LatestEvent?.IsPendingCompletion ?? false);

            _logger.LogInformation($"Found {visitsPendingCompletion.Count()} visits where the latest event is pending completion.");

            // Iterate events
            foreach (var visit in visitsPendingCompletion)
            {
                var e = visit.CompleteLatestEvent();

                _logger.LogInformation($"Completed event {e.GetType().Name} for visit {visit.Id}.");

                _logger.LogDebug($"{visit}");

                _mediator.Publish(new EmergencyDepartmentEventCompletedEvent(e));
            }
        }

        /// <summary>
        /// Processes the visits waiting to be seen in the ED.
        /// </summary>
        public void ProcessVisitsWaitingToBeSeen()
        {
            // Find visits waiting to be seen
            var visits = _emergencyDepartment.Visits.Where(v => v.IsWaitingToBeSeen);

            _logger.LogInformation($"Found {visits.Count()} visits waiting to be seen.");

            // Iterate visits by priority
            foreach (var visit in visits.OrderByDescending(v => v, new EmergencyDepartmentVisitComparer()))
            {
                _logger.LogInformation($"Searching for clinician for visit {visit.Id}.");

                // Find available clinician
                var clinician = _emergencyDepartment.GetAvailableClinician();

                // No clinicians are available, all remaining patients must wait
                if (clinician == null)
                {
                    _logger.LogInformation($"All clinicians are busy.");

                    break;
                }

                _logger.LogInformation($"Found clinician {clinician.ClinicianNumber}.");

                // Create the next ED event for the visit
                var e = visit.StartNextEvent(clinician);

                _logger.LogInformation($"Started event {e.GetType().Name} for visit {visit.Id}.");

                _logger.LogDebug($"Visit Details: {visit}");

                // Publish event
                _mediator.Publish(new EmergencyDepartmentEventStartedEvent(e));
            }
        }

        /// <summary>
        /// Generates a clinician.
        /// </summary>
        private Clinician GenerateClinician()
        {
            var clinicianNumber = $"C{new Random().Next(100000, 999999)}";
            var firstName = Faker.Name.First();
            var surname = Faker.Name.Last();

            var clinician = new Clinician(clinicianNumber, new Name(firstName, surname));

            return clinician;
        }

        /// <summary>
        /// Generates a patient.
        /// </summary>
        private static Patient GeneratePatient()
        {
            var nhsNumber = Faker.Identification.UkNhsNumber();
            var firstName = Faker.Name.First();
            var surname = Faker.Name.Last();
            var dateOfBirth = Faker.Identification.DateOfBirth();
            var street = Faker.Address.StreetAddress();
            var city = Faker.Address.City();
            var county = Faker.Address.UkCounty();
            var country = Faker.Address.UkCountry();
            var postcode = Faker.Address.UkPostCode();

            var patient = new Patient(
                nhsNumber, 
                new Name(firstName, surname), 
                dateOfBirth, 
                new Address(street, city, county, country, postcode));

            return patient;
        }
    }
}