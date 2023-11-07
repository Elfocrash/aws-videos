using Customers.Api.Contracts.Requests;
using Customers.Api.Mapping;
using Customers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Controllers;

[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("customers")]
    public async Task<IActionResult> Create([FromBody] CustomerRequest request)
    {
        var customer = request.ToCustomer();

        await _customerService.CreateAsync(customer);

        var customerResponse = customer.ToCustomerResponse();

        return CreatedAtAction("Get", new { customerResponse.Id }, customerResponse);
    }

    [HttpGet("customers/{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var customer = await _customerService.GetAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        var customerResponse = customer.ToCustomerResponse();
        return Ok(customerResponse);
    }
    
    [HttpGet("customers")]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        var customersResponse = customers.ToCustomersResponse();
        return Ok(customersResponse);
    }
    
    [HttpPut("customers/{id:guid}")]
    public async Task<IActionResult> Update(
        [FromMultiSource] UpdateCustomerRequest request)
    {
        var requestStarted = DateTime.UtcNow;
        var existingCustomer = await _customerService.GetAsync(request.Id);

        if (existingCustomer is null)
        {
            return NotFound();
        }

        var customer = request.ToCustomer();
        await _customerService.UpdateAsync(customer, requestStarted);

        var customerResponse = customer.ToCustomerResponse();
        return Ok(customerResponse);
    }
    
    [HttpDelete("customers/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await _customerService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
