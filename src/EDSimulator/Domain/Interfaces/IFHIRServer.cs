using Hl7.Fhir.Model;

namespace EDSimulator.Domain.Interfaces
{
    public interface IFHIRServer
    {
        Task<T> CreateResource<T>(T resource) where T : Resource;
        Task<T> GetResourceForId<T>(string id) where T : Resource;
        Task<T> UpdateResource<T>(T resource) where T : Resource;
    }
}
