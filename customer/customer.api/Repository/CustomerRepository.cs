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

        public CustomerRepository(ActivitySource activitySource)
        {

            string connection = Environment.GetEnvironmentVariable("MONGO_CONNECTION") ?? "mongodb://user:pwd@localhost:27017/admin";
            _client = new MongoClient(connection);
            _database = _client.GetDatabase("test_system");
            _mongoCollection = _database.GetCollection<Customer>("customers");
            _activitySource = activitySource;
        }

        public Customer GetByDocumentNumber(string documentNumber)
        {
            var activity = _activitySource.StartActivity("CustomerRepository.GetByDocumentNumber");
            using (activity)
            {
                return _mongoCollection.Find(c => c.DocumentNumber == documentNumber).FirstOrDefault();
            }
        }

        public IEnumerable<Customer> FindAll()
        {
            var activity = _activitySource.StartActivity("CustomerRepository.FindAll");
            using (activity)
            {
                return _mongoCollection.Find(c => c.DocumentNumber != "").ToList();
            }
        }

        public void InsertOne(Customer customer)
        {
            var activity = _activitySource.StartActivity("CustomerRepository.InsertOne");
            using (activity)
            {
                _mongoCollection.InsertOne(customer);
            }
        }

        public void ReplaceOne(Customer customer)
        {
            var activity = _activitySource.StartActivity("CustomerRepository.ReplaceOne");
            using (activity)
            {
                _mongoCollection.ReplaceOne(m => m._id == customer._id, customer);
            }
        }
    }
}
