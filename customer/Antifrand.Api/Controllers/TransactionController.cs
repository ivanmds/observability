using System.Diagnostics;
using System.Diagnostics.Metrics;
using Antifrand.Api.Model;
using Antifrand.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Antifrand.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private static List<Transaction> _transactions = new List<Transaction>();

        private readonly ILogger<TransactionController> _logger;
        private readonly Counter<int> _counter;
        private readonly TransactionRepository _transactionRepository;

        public TransactionController(ILogger<TransactionController> logger, Counter<int> counter, TransactionRepository transactionRepository)
        {
            _logger = logger;
            _counter = counter;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            _counter.Add(1);
            _logger.LogInformation("{ test: 'test' }");
            
            var transactions = _transactionRepository.FindAll();
            return transactions;
        }

        [HttpPut]
        public void Put(Transaction transaction)
        {
            _counter.Add(1);
            _transactionRepository.InsertOne(transaction);
            var jsonLog = JsonConvert.SerializeObject(transaction);
            _logger.LogInformation(jsonLog);
        }
    }
}
