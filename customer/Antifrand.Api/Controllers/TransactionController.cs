using System.Diagnostics;
using System.Diagnostics.Metrics;
using Antifrand.Api.Model;
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
        private readonly ActivitySource _activitySource;

        public TransactionController(ILogger<TransactionController> logger, Counter<int> counter)
        {
            _activitySource = new ActivitySource("customers");
            _logger = logger;
            _counter = counter;
        }

        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            _counter.Add(1);
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Get to transactions");
            using var activity = _activitySource.StartActivity("GetTransactions");
            activity?.SetTag("foo", 1);



            stopwatch.Stop();
            return _transactions;
        }

        [HttpPut]
        public void Put(Transaction customer)
        {
            _counter.Add(1);
            _transactions.Add(customer);
            var jsonLog = JsonConvert.SerializeObject(customer);
            _logger.LogInformation(jsonLog);
        }
    }
}
