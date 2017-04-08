using System.Collections.Generic;
using MongoDB.Bson; 
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Linq;

namespace HeartbeatService.Models
{
    public class MongoDbEndpointRepository : IEndpointRepository
    {
        private readonly MongoOptions _options;

        private readonly IMongoDatabase _db;

        private readonly IMongoClient _client;

     public MongoDbEndpointRepository(IOptions<MongoOptions> options)
        {
            _options = options.Value;

            _client = new MongoClient(_options.ConnectingString);

            _db = _client.GetDatabase(_options.DbName);
        }

        public void Add(Endpoint item)
        {
            var collection = _db.GetCollection<Endpoint>(_options.EndpointCollectionName);

            var elements = collection.Find(new BsonDocument()).ToListAsync().Result;

            if (elements.Any())
            {
                item.Id = elements.Max(n => n.Id) + 1;
            }
            else
            {
                item.Id = 1;
            }           

            collection.ReplaceOneAsync(x => x.Id.Equals(item.Id), item, new UpdateOptions
            {
                IsUpsert = true
            });
        }

        public IEnumerable<Endpoint> GetAll()
        {                        
            return _db.GetCollection<Endpoint>(_options.EndpointCollectionName).Find(new BsonDocument()).ToListAsync().Result;
        }

        public void Remove(int id)
        {
            var collection = _db.GetCollection<Endpoint>(_options.EndpointCollectionName);
            collection.DeleteOneAsync(x => x.Id.Equals(id));
        }
    }
}
