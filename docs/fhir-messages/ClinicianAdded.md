# Clinician Added

The following messages are sent to the FHIR server when a clinician is added to the emergency department.

## 1. Create Practitioner

### Sample Message

```xml
<Practitioner xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <identifier>
        <system value="https://fhir.nhs.uk/Id/sds-user-id" />
        <value value="<clinician-number>" />
    </identifier>
    <name>
        <family value="<surname>" />
        <given value="<first-name>" />
    </name>
</Practitioner>
```
