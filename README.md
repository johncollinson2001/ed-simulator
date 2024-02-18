# Emergency Department Simulator

This solution simulates an emergency care department.

When the simulation is started, an emergency care department is created and a number of clinicians
are added to the department. 

As the simulation progresses, patients admitted to the department and 
progressed through a typical ED workflow consisting of the following stages:

* Triage
* Assessment
* Treatment 
* Discharge

Random-ness is applied throughout the simulation, for example when generating how frequently 
a patient arrives, or how long a clinician requires to triage/assess/treat a patient.

The simluation integrates with a FHIR server. When events occur in the simulation, 
messages are sent to update the FHIR server.

⚠️ The simulation is currently pretty basic and crude, for example you may find patients are diagnosed
with a heart attack but treated with paracetamol 😄  

⚠️ HL7v2 messaging has recently been introduced to meet an urgent need. It has not yet been documented!  

## Background

This software was created to support teams in learning about FIHR. 

It could also be used to test a FHIR server, both for connectivity/behaviour
and volume/performance.

## Running the Application

Compile the application (using the .net cli or an IDE such as Visual Studio), open a command line and run the executable:

```
EDSimulator.exe -simulation-speed-multiplier 20 
                -number-of-clinicians 10 
                -size-of-population 500000 
                -population-wrecklessness 5
```

## Documentation

* [Configuration](./docs/Configuration.md)
* [Design](./docs/Design.md)
* [FHIR Server](./docs/FHIRServer.md)
* [FHIR Messages](./docs/FHIRMessages.md) 
