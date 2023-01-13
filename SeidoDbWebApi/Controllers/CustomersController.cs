using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DbModelsLib;
using DbCRUDReposLib;

namespace DbAppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private ICustomerRepository _repo;
        private ILogger<CustomersController> _logger;

        //GET: api/customers
        //GET: api/customers/?country={country}
        //Below are good practice decorators to use for a GET request
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(string country)
        {
            _logger.LogInformation("GetCustomers initiated");
            if (string.IsNullOrWhiteSpace(country))
            {
                var cus = await _repo.ReadAllAsync();

                _logger.LogInformation("GetCustomers returned {count} customers", cus.Count());
                return cus;
            }
            else
            {
                var cus = await _repo.ReadAllAsync();
                cus = cus.Where(cust => cust.Country == country);

                _logger.LogInformation("GetCustomers returned {count} customers in country {country}", cus.Count(), country);
                return cus;
            }
        }
        
        //GET: api/customers/id
        [HttpGet("{custId}", Name = nameof(GetCustomer))]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(string custId)
        {
            if (!Guid.TryParse(custId, out Guid custGuid))
            {
                return BadRequest("Guid format error");
            }
            Customer cust = await _repo.ReadAsync(custGuid);
            if (cust != null)
            {
                //cust is returned in the body
                return Ok(cust);
            }
            else
            {
                return NotFound();
            }
        }

        //PUT: api/customers/id
        //Body: Customer in Json
        [HttpPut("{custId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomer(string custId, [FromBody] Customer cust)
        {
            if (!Guid.TryParse(custId, out Guid custGuid))
            {
                return BadRequest("Guid format error");
            }
            if (custGuid != cust.CustomerID)
            {
                return BadRequest("Customer ID mismatch");               
            }

            cust = await _repo.UpdateAsync(cust);
            if (cust != null)
            {
                _logger.LogInformation("Updated customer {cusId}", custGuid);
                //Send an empty body response to confirm
                return new NoContentResult();
            }
            else
            {
                return NotFound();
            }
        }

        //DELETE: api/customers/id
        [HttpDelete("{custId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCustomer(string custId)
        {
            if (!Guid.TryParse(custId, out Guid custGuid))
            {
                return BadRequest("Guid format error");
            }

            Customer cust = await _repo.ReadAsync(custGuid);
            if (cust == null)
            {
                return NotFound();   
            }

            cust = await _repo.DeleteAsync(custGuid);
            if (cust != null)
            {
                _logger.LogInformation("Deleted customer {cusId}", custGuid);

                //Send an empty body response to confirm
                return new NoContentResult();
            }
            else
            {
                return BadRequest("Customer found but could not be deleted");
            }
        }

        //POST: api/customers
        //Body: Customer in Json
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer cust)
        {
            if (cust == null)
            {
                return BadRequest("No Customer");               
            }
            if (await _repo.ReadAsync(cust.CustomerID)  != null)
            {
                return BadRequest("Customer ID already existing");   
            }
            
            cust = await _repo.CreateAsync(cust);
            if (cust != null)
            {
                //201 created ok with url details to read newlys created customer
                return CreatedAtRoute(
                    
                    //Named Route in the HttpGet request
                    routeName: nameof(GetCustomer),  

                    //custId is the parameter in HttpGet
                    routeValues: new {custId = cust.CustomerID.ToString().ToLower()}, 

                    //Customer detail in the Body
                    value: cust);
            }
            else
            {
                return BadRequest("Could not create Customer");
            }
        }


        public CustomersController(ICustomerRepository repo, ILogger<CustomersController> logger)
        {
            _repo = repo;
            _logger = logger;

            _logger.LogInformation($"CustomersController started. Connected to {AppConfig.ThisConnection}");
        }
    }
}