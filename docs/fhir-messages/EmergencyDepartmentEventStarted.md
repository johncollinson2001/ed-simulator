# Emergency Department Event Started

When an event has started in the emergency department, different messages are sent 
depending on the type of event - e.g. triage/assessment/treatment/discharge.

The following sections list the messages which are sent for each type of ED event.

## Patient Triage Started

The following messages are sent to the FHIR server when a patient's triage is started.

### 1. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="in-progress" />
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
            <start value="<event-date-time>" />
        </period>
    </participant>
</Encounter>
```

## Patient Assessment Started

The following messages are sent to the FHIR server when a patient's assessment is started.

### 1. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="in-progress" />
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
            <start value="<event-date-time>" />
        </period>
    </participant>
</Encounter>
```

## Patient Treatment Started

The following messages are sent to the FHIR server when a patient's treatment is started.

### 1. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="in-progress" />
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
            <start value="<event-date-time>" />
        </period>
    </participant>
</Encounter>
```

## Patient Discharge Started

The following messages are sent to the FHIR server when a patient discharge has been started.

### 1. Update Encounter

#### Sample Message

```xml
<Encounter xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <status value="in-progress" />
    <participant>
        <type>
            <coding>
                <system value="http://terminology.hl7.org/CodeSystem/v3-ParticipationType" />
                <code value="DIS" />
                <display value="discharger" />
            </coding>
        </type>
        <individual>
            <reference value="Practitioner/<practitioner-unique-id>" />
        </individual>
        <period>
            <start value="<event-date-time>" />
        </period>
    </participant>
</Encounter>
```