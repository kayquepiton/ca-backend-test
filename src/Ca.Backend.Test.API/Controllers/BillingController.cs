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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ImportBillingAsync()
    {
        await _billingServices.ImportBillingFromExternalApiAsync();
        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(typeof(BillingResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBillingAsync([FromBody] BillingRequest request)
    {
        var response = await _billingServices.CreateAsync(request);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBillingByIdAsync(Guid id)
    {
        var response = await _billingServices.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BillingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBillingsAsync()
    {
        var response = await _billingServices.GetAllAsync();
        return Ok(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBillingAsync(Guid id, [FromBody] BillingRequest request)
    {
        var response = await _billingServices.UpdateAsync(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBillingAsync(Guid id)
    {
        await _billingServices.DeleteByIdAsync(id);
        return Ok();
    }

}
