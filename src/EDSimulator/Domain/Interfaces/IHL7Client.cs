namespace EDSimulator.Domain.Interfaces
{
    public interface IHL7Client
    {
        Task SendMessage(string message);
    }
}
