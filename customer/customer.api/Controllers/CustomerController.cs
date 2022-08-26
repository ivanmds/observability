using System.Diagnostics;
using System.Diagnostics.Metrics;
using customer.api.Model;
using customer.api.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace customer.api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static HttpClient _client = new HttpClient();

        private readonly ILogger<CustomerController> _logger;
        private readonly Counter<int> _counter;
        private readonly CustomerRepository _customerRepository;
        private readonly ActivitySource _activitySource;

        public CustomerController(ILogger<CustomerController> logger, Counter<int> counter, CustomerRepository customerRepository)
        {
            _activitySource = new ActivitySource("customers");
            _logger = logger;
            _counter = counter;
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            _counter.Add(1);

            _logger.LogInformation("Get to customers");

            var customers = _customerRepository.FindAll();

            return customers;
        }

        [HttpPut]
        public async Task Put(Customer customer)
        {
            _counter.Add(1);

            var customerFound = _customerRepository.GetByDocumentNumber(customer.DocumentNumber);
            if(customerFound == null)
                _customerRepository.InsertOne(customer);

            var response = await _client.GetAsync("http://localhost:5131/api/transaction");


            var jsonLog = JsonConvert.SerializeObject(customer);
            _logger.LogInformation(jsonLog);
        }
    }
}
