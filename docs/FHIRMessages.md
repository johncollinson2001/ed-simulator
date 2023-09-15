# FHIR Messages

Messages are sent to the FHIR server following a domain event. 

All messages are sent using the UK Core FHIR standard. See the following link
for information on UK Core: https://simplifier.net/hl7fhirukcorer4.

A domain event will exchange a number of messages with the server, as multiple FHIR 
resources may need to be created or updated to reflect the state of the emergency department.

See the following articles for details of the messages exchanged for each domain 
event:

* [Emergency Department Created](./fhir-messages/EmergencyDepartmentCreated.md)
* [Clinician Added](./fhir-messages/ClinicianAdded.md)
* [Emergency Department Visit Created](./fhir-messages/EmergencyDepartmentVisitCreated.md)
* [Emergency Department Event Started](./fhir-messages/EmergencyDepartmentEventStarted.md)
* [Emergency Department Event Completed](./fhir-messages/EmergencyDepartmentEventCompleted.md)

> :information_source: The emergency care department visit is concluded when the discharge
> event is completed.

## Links and Resources

### FHIR Profiles

* Encounter (UK Core) - https://simplifier.net/hl7fhirukcorer4/ukcore-encounter
* Patient (UK Core) - https://simplifier.net/hl7fhirukcorer4/ukcore-patient
* Practitioner (UK Core) - https://simplifier.net/hl7fhirukcorer4/ukcore-practitioner
* Location (UK Core) - https://simplifier.net/hl7fhirukcorer4/ukcore-location
* Organisation (UK Core) - https://simplifier.net/hl7fhirukcorer4/ukcore-organization
* Condition (UK Core) - https://simplifier.net/hl7fhirukcorer4/ukcore-condition
* Procedure (UK Core) - https://simplifier.net/hl7fhirukcorer4/ukcore-procedure