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
        private static int increment = 0;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> Get()
        {
            increment++;
            if (increment % 3 == 0)
                return NotFound();

            if (increment % 4 == 0)
                throw new Exception("Server error");


            _logger.LogInformation("Get to customers");

            return Ok(new List<Customer>());
        }

        //[HttpPut]
        //public async Task Put(Customer customer)
        //{
        //    var customerFound = _customerRepository.GetByDocumentNumber(customer.DocumentNumber);
        //    if(customerFound == null)
        //        _customerRepository.InsertOne(customer);

        //    //var response = await _client.GetAsync("http://localhost:5131/api/transaction");


        //    var jsonLog = JsonConvert.SerializeObject(customer);
        //    _logger.LogInformation(jsonLog);
        //}
    }
}
