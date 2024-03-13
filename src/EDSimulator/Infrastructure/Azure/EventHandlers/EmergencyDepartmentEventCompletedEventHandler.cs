using EDSimulator.Core.DomainEvents;
using EDSimulator.Core.Entities;
using EDSimulator.Core.Enums;
using EDSimulator.Core.Interfaces;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EDSimulator.Infrastructure.Azure.DomainEventHandlers
{
    public class EmergencyDepartmentEventCompletedEventHandler : INotificationHandler<EmergencyDepartmentEventCompletedEvent>
    {
        private readonly IFHIRServer _fhirServer;
        private readonly ILogger<EmergencyDepartmentEventCompletedEventHandler> _logger;

        public EmergencyDepartmentEventCompletedEventHandler(IFHIRServer fhirServer, ILogger<EmergencyDepartmentEventCompletedEventHandler> logger)
        {
            _fhirServer = fhirServer ?? throw new ArgumentNullException(nameof(fhirServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the emergency department created event.
        /// </summary>
        /// <param name="e">Contains all the relevant details of the event.</param>
        public async System.Threading.Tasks.Task Handle(EmergencyDepartmentEventCompletedEvent e, CancellationToken cancellationToken)
        {
            if (_fhirServer.IsDisabled)
                return;

            switch (e.Event)
            {
                case EmergencyDepartmentEventTriage:
                    await HandleTriageEvent(e);
                    break;
                
                case EmergencyDepartmentEventAssessment:
                    await HandleAssessmentEvent(e);
                    break;

                case EmergencyDepartmentEventTreatment:
                    await HandleTreatmentEvent(e);
                    break;

                case EmergencyDepartmentEventDischarge:
                    await HandleDischargeEvent(e);
                    break;

                default:
                    _logger.LogDebug($"HL7 message handler not implemented for event type {e.GetType().Name}.");
                    break;
            }
        }

        /// <summary>
        /// Handler for when a triage event has been completed.
        /// </summary>
        private async System.Threading.Tasks.Task HandleTriageEvent(EmergencyDepartmentEventCompletedEvent e)
        {
            var chiefComplaint = e.Event.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareChiefComplaint);
            var priority = e.Event.Coding.Single(c => c.CodesetType == CodesetType.Priority);


            // Create condition 
            // ...

            _logger.LogInformation($"Creating condition resource for visit {e.Event.Visit.ShortId}.");

            var condition = new Condition()
            {
                Id = chiefComplaint.Id.ToString(),
                Subject = new ResourceReference($"Patient/{e.Event.Visit.Patient.Id}"),
                Category = new List<CodeableConcept>()
                {
                    new CodeableConcept(
                        "http://terminology.hl7.org/CodeSystem/condition-category",
                        "encounter-diagnosis",
                        "Encounter Diagnosis")
                },
                Code = new CodeableConcept(
                        "http://snomed.info/sct",
                        chiefComplaint.Code,
                        chiefComplaint.Description)
            };


            await _fhirServer.CreateResource(condition);

            _logger.LogInformation($"Condition resource created successfully for visit {e.Event.Visit.ShortId}.");
            

            // Update encounter
            // ...

            _logger.LogInformation($"Updating encounter resource for visit {e.Event.Visit.ShortId}.");

            var encounter = new Encounter()
            {
                Id = e.Event.Visit.Id.ToString(),
                Status = Encounter.EncounterStatus.Triaged,
                Participant = new List<Encounter.ParticipantComponent>()
                {
                    new Encounter.ParticipantComponent()
                    {
                        Type = new List<CodeableConcept>()
                        {
                            new CodeableConcept(
                                "http://terminology.hl7.org/CodeSystem/v3-ParticipationType", 
                                "ATND", 
                                "attender")
                        },
                        Individual = new ResourceReference($"Practitioner/{e.Event.Clinician.Id}"),
                        Period = new Period()
                        {
                            End = e.Event.CompletionDateTime?.ToFhirDateTime(TimeZoneInfo.Utc.BaseUtcOffset)
                        },
                    }
                },
                Priority = new CodeableConcept(
                    "http://terminology.hl7.org/CodeSystem/v3-ActPriority",
                    priority.Code,
                    priority.Description),
                Diagnosis = new List<Encounter.DiagnosisComponent>()
                {
                    new Encounter.DiagnosisComponent()
                    {
                        Condition = new ResourceReference($"Condition/{chiefComplaint.Id}"),
                        Use = new CodeableConcept(
                            "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                            "CC",
                            "Chief complaint")
                    }
                }
            };

            await _fhirServer.UpdateResource(encounter);

            _logger.LogInformation($"Encounter resource updated successfully for visit {e.Event.Visit.ShortId}.");
        }

        /// <summary>
        /// Updates the encounter FHIR resource when a assessment event has been completed.
        /// </summary>
        private async System.Threading.Tasks.Task HandleAssessmentEvent(EmergencyDepartmentEventCompletedEvent e)
        {
            var diagnosis = e.Event.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareDiagnosis);


            // Create condition 
            // ...

            _logger.LogInformation($"Creating condition resource for visit {e.Event.Visit.Id}.");

            var condition = new Condition()
            {
                Id = diagnosis.Id.ToString(),
                Subject = new ResourceReference($"Patient/{e.Event.Visit.Patient.Id}"),
                Category = new List<CodeableConcept>()
                {
                    new CodeableConcept(
                        "http://terminology.hl7.org/CodeSystem/condition-category",
                        "encounter-diagnosis",
                        "Encounter Diagnosis")
                },
                Code = new CodeableConcept(
                        "http://snomed.info/sct",
                        diagnosis.Code,
                        diagnosis.Description)
            };


            await _fhirServer.CreateResource(condition);

            _logger.LogInformation($"Condition resource created successfully for visit {e.Event.Visit.ShortId}.");


            // Update encounter
            // ...

            _logger.LogInformation($"Updating encounter resource for visit {e.Event.Visit.ShortId}.");

            var encounter = new Encounter()
            {
                Id = e.Event.Visit.Id.ToString(),
                Status = Encounter.EncounterStatus.Triaged,
                Participant = new List<Encounter.ParticipantComponent>()
                {
                    new Encounter.ParticipantComponent()
                    {
                        Type = new List<CodeableConcept>()
                        {
                            new CodeableConcept(
                                "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                "ATND",
                                "attender")
                        },
                        Individual = new ResourceReference($"Practitioner/{e.Event.Clinician.Id}"),
                        Period = new Period()
                        {
                            End = e.Event.CompletionDateTime?.ToFhirDateTime(TimeZoneInfo.Utc.BaseUtcOffset)
                        },
                    }
                },
                Diagnosis = new List<Encounter.DiagnosisComponent>()
                {
                    new Encounter.DiagnosisComponent()
                    {
                        Condition = new ResourceReference($"Condition/{diagnosis.Id}"),
                        Use = new CodeableConcept(
                            "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                            "AD",
                            "Admission diagnosis")
                    }
                }
            };

            await _fhirServer.UpdateResource(encounter);

            _logger.LogInformation($"Encounter resource updated successfullyfor visit {e.Event.Visit.ShortId}.");
        }

        /// <summary>
        /// Updates the encounter FHIR resource when a treatment event has been completed.
        /// </summary>
        private async System.Threading.Tasks.Task HandleTreatmentEvent(EmergencyDepartmentEventCompletedEvent e)
        {
            var treatment = e.Event.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareTreatment);


            // Create condition 
            // ...

            _logger.LogInformation($"Creating procedure resource for visit {e.Event.Visit.ShortId}.");

            var procedure = new Procedure()
            {
                Id = treatment.Id.ToString(),
                Subject = new ResourceReference($"Patient/{e.Event.Visit.Patient.Id}"),
                Performed = new FhirDateTime(new DateTimeOffset(e.Event.CompletionDateTime!.Value)),
                Performer = new List<Procedure.PerformerComponent>()
                {
                    new Procedure.PerformerComponent()
                    {
                        Actor = new ResourceReference($"Practitioner/{e.Event.Clinician.Id}")
                    }
                },
                Code = new CodeableConcept(
                        "http://snomed.info/sct",
                        treatment.Code,
                        treatment.Description)
            };


            await _fhirServer.CreateResource(procedure);

            _logger.LogInformation($"Procedure resource created successfully for visit {e.Event.Visit.ShortId}.");


            // Update encounter
            // ...

            _logger.LogInformation($"Updating encounter resource for visit {e.Event.Visit.ShortId}.");

            var encounter = new Encounter()
            {
                Id = e.Event.Visit.Id.ToString(),
                Status = Encounter.EncounterStatus.Triaged,
                Participant = new List<Encounter.ParticipantComponent>()
                {
                    new Encounter.ParticipantComponent()
                    {
                        Type = new List<CodeableConcept>()
                        {
                            new CodeableConcept(
                                "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                "ATND",
                                "attender")
                        },
                        Individual = new ResourceReference($"Practitioner/{e.Event.Clinician.Id}"),
                        Period = new Period()
                        {
                            End = e.Event.CompletionDateTime?.ToFhirDateTime(TimeZoneInfo.Utc.BaseUtcOffset)
                        },
                    }
                },
                Diagnosis = new List<Encounter.DiagnosisComponent>()
                {
                    new Encounter.DiagnosisComponent()
                    {
                        Condition = new ResourceReference($"Procedure/{treatment.Id}")
                    }
                }
            };

            await _fhirServer.UpdateResource(encounter);

            _logger.LogInformation($"Encounter resource updated successfully for visit {e.Event.Visit.ShortId}.");
        }

        /// <summary>
        /// Updates the encounter FHIR resource when a discharge event has been completed.
        /// </summary>
        private async System.Threading.Tasks.Task HandleDischargeEvent(EmergencyDepartmentEventCompletedEvent e)
        {
            var dischargeMethod = e.Event.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareDischargeMethod);
            var dischargeStatus = e.Event.Coding.Single(c => c.CodesetType == CodesetType.EmergencyCareDischargeStatus);


            // Update encounter
            // ...

            _logger.LogInformation($"Updating encounter resource for visit {e.Event.Visit.ShortId}.");

            var encounter = new Encounter()
            {
                Id = e.Event.Visit.Id.ToString(),
                Status = Encounter.EncounterStatus.Finished,
                Participant = new List<Encounter.ParticipantComponent>()
                {
                    new Encounter.ParticipantComponent()
                    {
                        Type = new List<CodeableConcept>()
                        {
                            new CodeableConcept(
                                "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                "ATND",
                                "attender")
                        },
                        Individual = new ResourceReference($"Practitioner/{e.Event.Clinician.Id}"),
                        Period = new Period()
                        {
                            End = e.Event.CompletionDateTime?.ToFhirDateTime(TimeZoneInfo.Utc.BaseUtcOffset)
                        },
                    }
                }
            };

            // Add extensions
            // ...

            var dischargeMethodExtension = new Extension(
                "https://fhir.hl7.org.uk/StructureDefinition/Extension-UKCore-DischargeMethod",
                new CodeableConcept(
                    "https://fhir.hl7.org.uk/CodeSystem/UKCore-DischargeMethodEngland", 
                    dischargeMethod.Code, 
                    dischargeMethod.Description));

            encounter.Extension.Add(dischargeMethodExtension);

            var dischargeStatusExtension = new Extension(
                "https://fhir.hl7.org.uk/StructureDefinition/Extension-UKCore-EmergencyCareDischargeStatus",
                new CodeableConcept(
                    "http://snomed.info/sct",
                    dischargeStatus.Code,
                    dischargeStatus.Description));

            encounter.Extension.Add(dischargeStatusExtension);

            await _fhirServer.UpdateResource(encounter);

            _logger.LogInformation($"Encounter resource updated successfully for visit {e.Event.Visit.ShortId}.");
        }
    }
}