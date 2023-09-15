# Emergency Department Event Completed

When an event has been completed in the emergency department, different messages are sent 
depending on the type of event - e.g. triage/assessment/treatment/discharge.

The following sections list the messages which are sent for each type of ED event.

## Patient Triage Completed

The following messages are sent to the FHIR server when a patient's triage is completed.

### 1. Create Condition

#### Sample Message

```xml
<Condition xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <category>
        <coding>
            <system value="http://terminology.hl7.org/CodeSystem/condition-category" />
            <code value="encounter-diagnosis" />
            <display value="Encounter Diagnosis" />
        </coding>
    </category>
    <code>
        <coding>
            <system value="http://snomed.info/sct" />
            <code value="<condition-snomed-code>" />
            <display value="<condition-snomed-description>" />
        </coding>
    </code>
    <subject>
        <reference value="Patient/<patient-unique-id>" />
    </subject>
</Condition>
```

### 2. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="triaged" />
    <participant>
        <type>
            <coding>
                <system value="http://terminology.hl7.org/CodeSystem/v3-ParticipationType" />
                <code value="ATND" />
                <display value="attender" />
            </coding>
        </type>
        <individual>
            <reference value="Practitioner/<practitioner-unique-id>" />
        </individual>
        <period>
            <end value="<event-date-time>" />
        </period>
    </participant>
    <priority>
        <system value="http://terminology.hl7.org/CodeSystem/v3-ActPriority" />
        <code value="<priority-code>" />
        <display value="<priority-description>" />
    </priority>
    <diagnosis>
        <condition>
            <reference value="Condition/<condition-unique-id>" />
        </condition>
        <use>
            <coding>
                <system value="http://terminology.hl7.org/CodeSystem/diagnosis-role" />
                <code value="CC" />
                <display value="Chief complaint" />
            </coding>
        </use>
    </diagnosis>
</Encounter>
```

## Patient Assessment Completed

The following messages are sent to the FHIR server when a patient's assessment is completed.

### 1. Create Condition

#### Sample Message

```xml
<Condition xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <category>
        <coding>
            <system value="http://terminology.hl7.org/CodeSystem/condition-category" />
            <code value="encounter-diagnosis" />
            <display value="Encounter Diagnosis" />
        </coding>
    </category>
    <code>
        <coding>
            <system value="http://snomed.info/sct" />
            <code value="<condition-snomed-code>" />
            <display value="<condition-snomed-description>" />
        </coding>
    </code>
    <subject>
        <reference value="Patient/<patient-unique-id>" />
    </subject>
</Condition>
```

### 2. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="triaged" />
    <participant>
        <type>
            <coding>
                <system value="http://terminology.hl7.org/CodeSystem/v3-ParticipationType" />
                <code value="ATND" />
                <display value="attender" />
            </coding>
        </type>
        <individual>
            <reference value="Practitioner/<practitioner-unique-id>" />
        </individual>
        <period>
            <end value="<event-date-time>" />
        </period>
    </participant>
    <diagnosis>
        <condition>
            <reference value="Condition/<condition-unique-id>" />
        </condition>
        <use>
            <coding>
                <system value="http://terminology.hl7.org/CodeSystem/diagnosis-role" />
                <code value="AD" />
                <display value="Admission diagnosis" />
            </coding>
        </use>
    </diagnosis>
</Encounter>
```

## Patient Treatment Completed

The following messages are sent to the FHIR server when a patient's treatment is completed.

### 1. Create Procedure

#### Sample Message

```xml
<Procedure xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="completed" />
    <code>
        <coding>
            <system value="http://snomed.info/sct" />
            <code value="<procedure-snomed-code>" />
            <display value="<procedure-snomed-description" />
        </coding>
    </code>
    <subject>
        <reference value="Patient/<patient-unique-id>" />
    </subject>
    <performedDateTime value="<procedure-date-time>" />
</Procedure>
```

### 2. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="triaged" />
    <participant>
        <type>
            <coding>
                <system value="http://terminology.hl7.org/CodeSystem/v3-ParticipationType" />
                <code value="ATND" />
                <display value="attender" />
            </coding>
        </type>
        <individual>
            <reference value="Practitioner/<practitioner-unique-id>" />
        </individual>
        <period>
            <end value="<event-date-time>" />
        </period>
    </participant>
    <diagnosis>
        <condition>
            <reference value="Procedure/<procedure-unique-id>" />
        </condition>
    </diagnosis>
</Encounter>
```

## Patient Discharge Completed

The following messages are sent to the FHIR server when a patient discharge is completed.

### 1. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="finished" />
    <participant>
        <type>
            <coding>
                <system value="http://terminology.hl7.org/CodeSystem/v3-ParticipationType" />
                <code value="ATND" />
                <display value="attender" />
            </coding>
        </type>
        <individual>
            <reference value="Practitioner/<practitioner-unique-id>" />
        </individual>
        <period>
            <end value="<event-date-time>" />
        </period>
    </participant>
    <period>
        <end value="<event-date-time>" />
    </period>
    <extension url="https://fhir.hl7.org.uk/StructureDefinition/Extension-UKCore-DischargeMethod">
        <valueCodeableConcept>
            <coding>
                <system value="https://fhir.hl7.org.uk/CodeSystem/UKCore-DischargeMethodEngland" />
                <code value="<discharge-method-code>" />
                <display value="<discharge-method-description>" />
            </coding>
        </valueCodeableConcept>
    </extension>
    <extension url="https://fhir.hl7.org.uk/StructureDefinition/Extension-UKCore-EmergencyCareDischargeStatus">
        <valueCodeableConcept>
            <coding>
                <system value="http://snomed.info/sct" />
                <code value="<discharge-status-snomed-code>" />
                <display value="<discharge-status-snomed-description>" />
            </coding>
        </valueCodeableConcept>
    </extension>
</Encounter>
```