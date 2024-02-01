using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace EDSimulator.Core.Interfaces
{
    public interface IFHIRServer
    {
        bool IsDisabled { get; }
        Task CreateResource<T>(T resource) where T : Resource;
        Task UpdateResource<T>(T resource) where T : Resource;
    }
}
