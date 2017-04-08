using System.Collections.Generic;

namespace HeartbeatService.Models
{
    public class InMemoryEndpointRepository : IEndpointRepository
    {
        private static int counter = 1;

        private List<Endpoint> _internalContainer = new List<Endpoint>();

        public IEnumerable<Endpoint> GetAll()
        {
            return _internalContainer;
        }

        public void Add(Endpoint item)
        {
            item.Id = counter++;
            _internalContainer.Add(item);
        }

        public void Remove(int id)
        {
            var item = _internalContainer.Find(n => n.Id == id);
            if (item != null)
            {
                _internalContainer.Remove(item);
            }
        }
    }
}
