namespace EDSimulator.Infrastructure.HL7
{
    public class HL7Configuration
    {
        public bool Disabled { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public Dictionary<string, string> HttpHeaders { get; set; } = new Dictionary<string, string>();
        public string VisitCreatedMessageType { get; set; } = string.Empty;
        public string SendingOrganisation { get; set; } = string.Empty;
        public string SendingApplication { get; set; } = string.Empty;
        public string ReceivingOrganisation { get; set; } = string.Empty;
        public string ReceivingApplication { get; set; } = string.Empty;
    }
}
