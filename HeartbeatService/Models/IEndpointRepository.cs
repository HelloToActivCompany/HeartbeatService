using System.Collections.Generic;

namespace HeartbeatService.Models
{
    public interface IEndpointRepository
    {
        IEnumerable<Endpoint> GetAll();
        void Add(Endpoint item);
        void Remove(int id);
    }
}
