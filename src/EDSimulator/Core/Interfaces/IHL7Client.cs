namespace EDSimulator.Core.Interfaces
{
    public interface IHL7Client
    {
        Task SendMessage(string message);
    }
}
