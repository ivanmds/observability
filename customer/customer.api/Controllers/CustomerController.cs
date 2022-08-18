using System.Diagnostics;
using customer.api.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace customer.api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static List<Customer> _customers = new List<Customer>();
        private readonly ILogger<CustomerController> _logger;
        private readonly ActivitySource _activitySource;


        public CustomerController(ILogger<CustomerController> logger)
        {

            _activitySource = new ActivitySource("customers");
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            _logger.LogInformation("Get to customers");
            using var activity = _activitySource.StartActivity("GetCustomers");
            activity?.SetTag("foo", 1);

            return _customers;
        }

        [HttpPut]
        public void Put(Customer customer)
        {
            _customers.Add(customer);
            var jsonLog = JsonConvert.SerializeObject(customer);
            _logger.LogInformation(jsonLog);
        }
    }
}
