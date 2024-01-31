namespace EDSimulator.Infrastructure.Azure
{
    public class FHIRServerConfiguration
    {
        public bool Enabled { get; set; }
        public string Endpoint { get; set; } = string.Empty;
    }
}
