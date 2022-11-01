using System.Diagnostics;
using Bankly.Sdk.Opentelemetry.Trace;
using customer.api.Model;
using MongoDB.Driver;

namespace customer.api.Repository
{
    public class CustomerRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Customer> _mongoCollection;
        private readonly ITraceService _traceService;

        public CustomerRepository(ITraceService traceService)
        {
            string connection = Environment.GetEnvironmentVariable("MONGO_CONNECTION") ?? "mongodb://userdoc:pwd@localhost:27017/admin";
            _client = new MongoClient(connection);
            _database = _client.GetDatabase("test_system");
            _mongoCollection = _database.GetCollection<Customer>("customers");
            _traceService = traceService;
        }

        public Customer GetByDocumentNumber(string documentNumber)
        {
            using (_traceService.GetActivitySource().StartActivity("CustomerRepository.GetByDocumentNumber"))
                return _mongoCollection.Find(c => c.DocumentNumber == documentNumber).FirstOrDefault();
        }


        public IEnumerable<Customer> FindAll()
        {
            using (_traceService.GetActivitySource().StartActivity("CustomerRepository.FindAll"))
                return new Customer[] { new Customer { DocumentNumber = "test123456" } };
        }

        public void InsertOne(Customer customer)
        {
            using (_traceService.GetActivitySource().StartActivity("CustomerRepository.InsertOne"))
                _mongoCollection.InsertOne(customer);
        }

        public void ReplaceOne(Customer customer)
        {
            using (_traceService.GetActivitySource().StartActivity("CustomerRepository.ReplaceOne"))
                _mongoCollection.ReplaceOne(m => m._id == customer._id, customer);
        }
    }
}
