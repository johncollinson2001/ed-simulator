# Emergency Department Simulator

> :warning: This solution is in active development, it may not be in a useable state right now!

This solution simulates an emergency care department.

When the simulation is started, an emergency care department is created and a number of clinicians
added to the department. 

As the simulation progresses patients admitted to the department and 
progressed through a typical ED workflow consisting of the following stages:

* Triage
* Assessment
* Treatment 
* Discharge

Random-ness is applied throughout the simulation, for example when generating how frequently 
a patient arrives, or how long a clinician requires to triage/assess/treat a patient.

The simluation integrates with a FHIR server. When events occur in the simulation, 
messages are sent to update the state of the FHIR server.

## Background

This software was created to support a healthcare hackathon to help teams 
learn about FIHR. The teams developed solutions that integrated with a FHIR server, while this 
software ran in the background to update the server and simulate a real-life scenario.

It could also be used to test a FHIR server, both for connectivity/behaviour
and volume/performance.

## Running the Application

Once the application is built, open a command line and run the executable:

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
