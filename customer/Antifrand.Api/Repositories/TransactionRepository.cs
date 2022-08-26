using System.Diagnostics;
using Antifrand.Api.Model;
using MongoDB.Driver;

namespace Antifrand.Api.Repositories
{
    public class TransactionRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Transaction> _mongoCollection;
        private readonly ActivitySource _activitySource;

        public TransactionRepository(ActivitySource activitySource)
        {
            _client = new MongoClient("mongodb://user:pwd@localhost:27017/admin");
            _database = _client.GetDatabase("test_system");
            _mongoCollection = _database.GetCollection<Transaction>("transactions");
            _activitySource = activitySource;
        }

        public IEnumerable<Transaction> FindAll()
        {
            var activity = _activitySource.StartActivity("TransactionRepository.FindAll");
            using (activity)
            {
                return _mongoCollection.Find(c => c.Amount != "").ToList();
            }
        }

        public void InsertOne(Transaction transaction)
        {
            var activity = _activitySource.StartActivity("TransactionRepository.InsertOne");
            using (activity)
            {
                _mongoCollection.InsertOne(transaction);
            }
        }
    }
}
