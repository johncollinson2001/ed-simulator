using EDSimulator.Core.DomainEvents;
using EDSimulator.Core.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EDSimulator.Infrastructure.Azure.DomainEventHandlers
{
    public class EmergencyDepartmentVisitCreatedEventHandler : INotificationHandler<EmergencyDepartmentVisitCreatedEvent>
    {
        private readonly IFHIRServer _fhirServer;
        private readonly ILogger<EmergencyDepartmentVisitCreatedEventHandler> _logger;

        public EmergencyDepartmentVisitCreatedEventHandler(IFHIRServer fhirServer, ILogger<EmergencyDepartmentVisitCreatedEventHandler> logger)
        {
            _fhirServer = fhirServer ?? throw new ArgumentNullException(nameof(fhirServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the emergency department created event.
        /// </summary>
        /// <param name="e">Contains all the relevant details of the event.</param>
        public async System.Threading.Tasks.Task Handle(EmergencyDepartmentVisitCreatedEvent e, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling event {e.GetType().Name}...");

            // Create patient
            await CreatePatient(e);

            // Create encounter
            await CreateEncounter(e);

            _logger.LogInformation($"{e.GetType().Name} handled successfully.");
        }

        /// <summary>
        /// Creates the patient FHIR resource.
        /// </summary>
        private async System.Threading.Tasks.Task CreatePatient(EmergencyDepartmentVisitCreatedEvent e)
        {
            _logger.LogInformation($"Creating patient resource.");

            var patient = new Hl7.Fhir.Model.Patient()
            {
                Id = e.Visit.Patient.Id.ToString(),
                Identifier = new List<Identifier>()
                {
                    new Identifier()
                    {
                        Use = Identifier.IdentifierUse.Official,
                        System = "https://fhir.nhs.uk/Id/nhs-number",
                        Value = e.Visit.Patient.NHSNumber
                    }
                },
                Name = new List<HumanName>()
                {
                    new HumanName()
                    {
                        Use = HumanName.NameUse.Official,
                        Family = e.Visit.Patient.Name.Surname,
                        Given = new List<string>()
                        {
                            e.Visit.Patient.Name.FirstName
                        }
                    }
                },
                BirthDate = e.Visit.Patient.DateOfBirth.ToFhirDate(),
                Address = new List<Address>()
                {
                    new Address() {
                        Line = new List<string>()
                        {
                            e.Visit.Patient.Address.Street
                        },
                        City = e.Visit.Patient.Address.City,
                        State = e.Visit.Patient.Address.County,
                        PostalCode = e.Visit.Patient.Address.Postcode,
                        Country = e.Visit.Patient.Address.Country
                    }
                }
            };

            // Add extensions
            // ...

            var nhsNumberStatusExtension = new Extension(
                "https://fhir.hl7.org.uk/StructureDefinition/Extension-UKCore-NHSNumberVerificationStatus",
                new CodeableConcept("https://fhir.hl7.org.uk/CodeSystem/UKCore-NHSNumberVerificationStatusEngland", "01", "Number present and verified"));

            patient.Identifier[0].Extension.Add(nhsNumberStatusExtension);

            await _fhirServer.CreateResource(patient);

            _logger.LogInformation($"Patient resource created successfully.");
        }

        /// <summary>
        /// Creates the encounter FHIR resource.
        /// </summary>
        private async System.Threading.Tasks.Task CreateEncounter(EmergencyDepartmentVisitCreatedEvent e)
        {
            _logger.LogInformation($"Creating encounter resource.");

            var encounter = new Encounter()
            {
                Id = e.Visit.Id.ToString(),
                Identifier = new List<Identifier>()
                {
                    new Identifier()
                    {
                        System = "https://tools.ietf.org/html/rfc4122",
                        Value = e.Visit.Id.ToString()
                    }
                },
                Status = Encounter.EncounterStatus.Arrived,
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "EMER", "emergency"),
                Type = new List<CodeableConcept>()
                {
                    new CodeableConcept("http://snomed.info/sct", "319981000000104", "Seen in urgent care centre")
                },
                ServiceType = new CodeableConcept("http://snomed.info/sct", "310000008", "Accident and Emergency service"),
                Subject = new ResourceReference($"Patient/{e.Visit.Patient.Id}"),
                Period = new Period()
                {
                    Start = e.Visit.StartDateTime.ToFhirDateTime(TimeZoneInfo.Utc.BaseUtcOffset)
                },
                ReasonCode = new List<CodeableConcept>()
                {
                    new CodeableConcept("http://snomed.info/sct", "182813001", "Emergency treatment")
                },
                Location = new List<Encounter.LocationComponent>()
                {
                    new Encounter.LocationComponent()
                    {
                        Location = new ResourceReference($"Location/{e.Visit.EmergencyDepartment.Id}")
                    }
                },
                ServiceProvider = new ResourceReference($"Organization/{e.Visit.EmergencyDepartment.Id}")
            };

            await _fhirServer.CreateResource(encounter);

            _logger.LogInformation($"Encounter resource created successfully.");
        }
    }
}