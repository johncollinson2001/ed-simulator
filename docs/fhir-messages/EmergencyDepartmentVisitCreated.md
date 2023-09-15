# Visit Created

The following messages are sent to the FHIR server when a patient visits the emergency department.

## 1. Create Patient

### Sample Message

```xml
<Patient xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <identifier>
        <extension url="https://fhir.hl7.org.uk/StructureDefinition/Extension-UKCore-NHSNumberVerificationStatus">
            <valueCodeableConcept>
                <coding>
                    <system value="https://fhir.hl7.org.uk/CodeSystem/UKCore-NHSNumberVerificationStatusEngland" />
                    <code value="01" />
                    <display value="Number present and verified" />
                </coding>
            </valueCodeableConcept>
        </extension>
        <system value="https://fhir.nhs.uk/Id/nhs-number" />
        <value value="<nhs-number>" />
    </identifier>
    <name>
        <use value="official" />
        <family value="<surname>" />
        <given value="<first-name>" />
    </name>
    <birthDate value="<date-of-birth>" />
    <address>
        <postalCode value="<postcode>" />
    </address>
</Patient>
```

## 2. Create Encounter

### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <identifier>
        <system value="https://tools.ietf.org/html/rfc4122" />
        <value value="<unique-id>" />
    </identifier>
    <status value="arrived" />
    <class>
        <system value="http://terminology.hl7.org/CodeSystem/v3-ActCode" />
        <code value="EMER" />
        <display value="emergency" />
    </class>
    <type>
        <coding>
            <system value="http://snomed.info/sct" />
            <code value="319981000000104" />
            <display value="Seen in urgent care centre" />
        </coding>
    </type>
    <serviceType>
        <coding>
            <system value="http://snomed.info/sct" />
            <code value="310000008" />
            <display value="Accident and Emergency service" />
        </coding>
    </serviceType>
    <subject>
        <reference value="Patient/<patient-unique-id>" />
    </subject>
    <period>
        <start value="<start-date-time>" />
    </period>
    <reasonCode>
        <coding>
            <system value="http://snomed.info/sct" />
            <code value="182813001" />
            <display value="Emergency treatment" />
        </coding>
    </reasonCode>
    <location>
        <location>
            <reference value="Location/<location-unique-id>" />
        </location>
    </location>
    <serviceProvider>
        <reference value="Organization/<organisation-unique-id>" />
    </serviceProvider>
</Encounter>
```
