# Configuration

## Simulation Configuration

The following configuration related to the flow of the simulation can be specified in both appsettings.json, and as command line arguments:

> :information_source: Command line arguments will override the configuration in appsettings.json.

* Simulation Speed Multiplier
  
  The speed at which the simulation will run. A value of 1 means the simulation will run in real time.
  A value of 2 means it will run at twice the speed of real time.

* Number of Clinicians

  The number of clinians working in the emergency department.

* Size of Population

  The size of the population. This has a direct impact on the interval at which 
  patients arrive at the emergency department. A population of 500k will mean patients
  arrive every 5 minutes.

* Population Wrecklessness

  The wrecklessness of the population. This has an impact on how many patients
  may arrive at one time. A value of 10, means that a random number of patients 
  between 1 and 10 will arrive.

### Example Configuration in appsettings.json

```json
{
  "SimulationSpeedMultiplier": 20,
  "NumberOfClinicians": 10,
  "SizeOfPopulation": 500000,
  "PopulationWrecklessness": 5
}
```

### Example Command Line Arguments

```
EDSimulator.exe -simulation-speed-multiplier 20 
                -number-of-clinicians 10 
                -size-of-population 500000 
                -population-wrecklessness 5
```

## FHIR Server Configuration

[See the following page for details on FHIR server configuration.](./FHIRServer.md)
