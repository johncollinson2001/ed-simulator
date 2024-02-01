namespace EDSimulator.Core.Interfaces
{
    public interface IHL7Client
    {
        bool IsDisabled { get; }
        Task SendMessage(string message);
    }
}
