namespace EDSimulator
{
    public class EDSimulatorConfiguration
    {
        public int SimulationSpeedMultiplier { get; set; } = 1;
        public int NumberOfClinicians { get; set; } = 10;
        public int SizeOfPopulation { get; set; } = 500000;
        public int PopulationWrecklessness { get; set; } = 5;
    }
}