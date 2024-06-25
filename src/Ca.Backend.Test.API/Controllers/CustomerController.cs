using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ca.Backend.Test.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerServices _customerServices;

    public CustomerController(ICustomerServices customerServices)
    {
        _customerServices = customerServices;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerRequest request)
    {
        var response = await _customerServices.CreateAsync(request);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
    {
        var response = await _customerServices.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCustomersAsync()
    {
        var response = await _customerServices.GetAllAsync();
        return Ok(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomerAsync(Guid id, [FromBody] CustomerRequest request)
    {
        var response = await _customerServices.UpdateAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomerAsync(Guid id)
    {
        await _customerServices.DeleteByIdAsync(id);
        return Ok();
    }
}
