using System.Diagnostics;
using customer.api.Model;
using MongoDB.Driver;

namespace customer.api.Repository
{
    public class CustomerRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Customer> _mongoCollection;
        private readonly ActivitySource _activitySource;

        public CustomerRepository()
        {
            _activitySource = new ActivitySource("CustomerRepository");
            _client = new MongoClient("mongodb://user:pwd@localhost:27017/admin");
            _database = _client.GetDatabase("test_system");
            _mongoCollection = _database.GetCollection<Customer>("customers");
        }

        public Customer GetByDocumentNumber(string documentNumber)
        {
            using var activity = _activitySource.StartActivity("GetByDocumentNumber");
            return _mongoCollection.Find(c => c.DocumentNumber == documentNumber).FirstOrDefault();
        }

        public IEnumerable<Customer> FindAll()
        {
            using var activity = _activitySource.StartActivity("FindAll");
            return _mongoCollection.Find(c => c.DocumentNumber != "").ToList();
        }

        public void InsertOne(Customer customer)
        {
            using var activity = _activitySource.StartActivity("InsertOne");
            _mongoCollection.InsertOne(customer);
        }

        public void ReplaceOne(Customer customer)
        {
            using var activity = _activitySource.StartActivity("ReplaceOne");
            _mongoCollection.ReplaceOne(m => m._id == customer._id, customer);
        }
    }
}
