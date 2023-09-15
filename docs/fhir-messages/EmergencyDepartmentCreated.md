# Emergency Department Created

The following messages are sent to the FHIR server when the emergency department is created.

## 1. Create Organisation

### Sample Message

```xml
<Organization xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <identifier>
        <use value="official" />
        <system value="https://fhir.nhs.uk/Id/ods-organization-code" />
        <value value="RBD" />
    </identifier>
    <name value="DORSET COUNTY HOSPITAL NHS FOUNDATION TRUST" />
    <address>
        <line value="DORSET COUNTY HOSPITAL" />
        <line value="WILLIAMS AVENUE" />
        <city value="DORCHESTER" />
        <postalCode value="DT1 2JY" />
        <country value="ENGLAND" />
    </address>
</Organization>
```

## 2. Create Location

### Sample Message

```xml
<Location xmlns="http://hl7.org/fhir">
    <id value="<unique-id>" />
    <identifier>
        <system value="https://fhir.nhs.uk/Id/ods-site-code" />
        <value value="RBD01" />
    </identifier>
    <name value="DORSET COUNTY HOSPITAL EMERGENCY DEPARTMENT" />
    <type>
        <coding>
            <system value="http://terminology.hl7.org/CodeSystem/v3-RoleCode" />
            <code value="ER" />
            <display value="Emergency room" />
        </coding>
    </type>
    <address>
        <line value="EMERGENCY DEPARTMENT" />
        <line value="DORSET COUNTY HOSPITAL" />
        <line value="WILLIAMS AVENUE" />
        <city value="DORCHESTER" />
        <postalCode value="DT1 2JY" />
        <country value="ENGLAND" />
    </address>
    <managingOrganization>
        <reference value="Organization/<unique-id>" />
    </managingOrganization>
</Location>
```