﻿using EDSimulator;
using EDSimulator.Core;
using EDSimulator.Core.Interfaces;
using EDSimulator.Infrastructure.Azure;
using EDSimulator.Infrastructure.HL7;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<EDSimulatorBackgroundService>();

        // Mediatr to listen to domain events...
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        // Other dependencies...
        services.AddTransient<IEmergencyDepartmentService, EmergencyDepartmentService>();
        services.AddSingleton<IFHIRServer, AzureAPIForFHIR>();
        services.AddSingleton<IHL7Client, HL7Client>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.AddSimpleConsole(formatterOptions =>
        {
            formatterOptions.SingleLine = true;
        });
    })
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        configuration.AddCommandLine(source =>
        {
            source.Args = args;
            source.SwitchMappings = new Dictionary<string, string>()
            {
                { "-simulation-speed-multiplier", "SimulationSpeedMultiplier" },
                { "-number-of-clinicians", "NumberOfClinicians" },
                { "-size-of-population", "SizeOfPopulation" },
                { "-population-wrecklessness", "PopulationWrecklessness" },
            };
        });

        configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
    });

var host = builder.Build();

host.Run();