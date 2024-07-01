using Ca.Backend.Test.API.Models.Response.Api;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ca.Backend.Test.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IBillingServices _billingServices;

    public BillingController(IBillingServices billingServices)
    {
        _billingServices = billingServices;
    }

    [HttpPost("importFromExternalApi")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportBillingAsync()
    {
        await _billingServices.ImportBillingFromExternalApiAsync();
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(GenericHttpResponse<BillingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBillingAsync([FromBody] BillingRequest request)
    {
        var response = await _billingServices.CreateAsync(request);
        return Ok(new GenericHttpResponse<BillingResponse>
        {
            Data = response
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<BillingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBillingByIdAsync(Guid id)
    {
        var response = await _billingServices.GetByIdAsync(id);
        return Ok(new GenericHttpResponse<BillingResponse>
        {
            Data = response
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(GenericHttpResponse<IEnumerable<BillingResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllBillingsAsync()
    {
        var response = await _billingServices.GetAllAsync();
        return Ok(new GenericHttpResponse<IEnumerable<BillingResponse>>
        {
            Data = response
        });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<BillingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBillingAsync(Guid id, [FromBody] BillingRequest request)
    {
        var response = await _billingServices.UpdateAsync(id, request);
        return Ok(new GenericHttpResponse<BillingResponse>
        {
            Data = response
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBillingAsync(Guid id)
    {
        await _billingServices.DeleteByIdAsync(id);
        return NoContent();
    }

}
