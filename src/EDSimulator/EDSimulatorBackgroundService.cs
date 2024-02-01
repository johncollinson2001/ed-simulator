using EDSimulator.Core;
using EDSimulator.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EDSimulator
{
    public sealed class EDSimulatorBackgroundService : BackgroundService
    {
        private readonly IEmergencyDepartmentService _emergencyDepartmentService;
        private readonly ILogger<EDSimulatorBackgroundService> _logger;
        private readonly Random _random = new Random();

        /// <summary>
        /// Patient arrival calculation is based on patients arriving every 10 minutes for a population of 500k.
        /// Randomness is applied to this value when calculating when patients will next arrive.
        /// </summary>
        private int PatientArrivalInterval => 5000000 / Configuration.SizeOfPopulation;

        /// <summary>
        /// App config
        /// </summary>
        public static EDSimulatorConfiguration Configuration { get; } = new();

        /// <summary>
        /// A really simple approach to speed up (or slow down) the time of the simulation. 
        /// Think Sim City and the fast forward button :-)
        /// </summary>
        public static DateTime SimulationDateTime => DateTime.Now + (DateTime.Now - Process.GetCurrentProcess().StartTime) * Configuration.SimulationSpeedMultiplier;

        /// <summary>
        /// Construct.
        /// </summary>
        public EDSimulatorBackgroundService(IEmergencyDepartmentService emergencyDepartmentService, ILogger<EDSimulatorBackgroundService> logger, IConfiguration configuration)
        {
            _emergencyDepartmentService = emergencyDepartmentService ?? throw new ArgumentNullException(nameof(emergencyDepartmentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            configuration.Bind(Configuration);
        }

        /// <summary>
        /// Executes the background service.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                var logMessage = string.Concat(
                    "ED Simulator background service has started with the following configuration:\n",
                    $"\tSimulation speed multiplier: {Configuration.SimulationSpeedMultiplier}\n",
                    $"\tNumber of clinicians: {Configuration.NumberOfClinicians}\n",
                    $"\tSize of population: {Configuration.SizeOfPopulation}\n",
                    $"\tPopulation wrecklessness: {Configuration.PopulationWrecklessness}"
                );

                _logger.LogInformation(logMessage);

                // Add clinicians to the ED
                for (var i = 0; i < Configuration.NumberOfClinicians; i++)
                {
                    _emergencyDepartmentService.AddClinician();
                }

                // Stores when the next set of patients will arrive
                var patientsArrivingOn = SimulationDateTime;

                while (true)
                {
                    _emergencyDepartmentService.UpdateState();

                    // Have patients arrived?
                    if (patientsArrivingOn <= SimulationDateTime)
                    {
                        // How many patients arrived? Depends how wreckless the population is!!
                        var numberOfPatientsArrived = _random.Next(1, Configuration.PopulationWrecklessness + 1);

                        _logger.LogInformation($"{numberOfPatientsArrived} patients have arrived at the department.");

                        // Create visits
                        for (var i = 0; i < numberOfPatientsArrived; i++)
                            _emergencyDepartmentService.CreateVisit();

                        // Store when patients will next arrive
                        var minutesUntilPatientsNextArrive = _random.Next(0, PatientArrivalInterval * 2);

                        patientsArrivingOn = patientsArrivingOn.AddMinutes(minutesUntilPatientsNextArrive);

                        _logger.LogInformation($"Patients will next arrive at {patientsArrivingOn}.");
                    }

                    Thread.Sleep(500);
                }
            });
        }
    }
}