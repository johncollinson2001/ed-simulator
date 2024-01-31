namespace EDSimulator.Infrastructure.Azure
{
    public class FHIRServerConfiguration
    {
        public bool Disabled { get; set; }
        public string Endpoint { get; set; } = string.Empty;
    }
}
