using Ca.Backend.Test.API.Models.Response.Api;
using Ca.Backend.Test.Application.Models.Request;
using Ca.Backend.Test.Application.Models.Response;
using Ca.Backend.Test.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ca.Backend.Test.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductServices _productServices;

    public ProductController(IProductServices productServices)
    {
        _productServices = productServices;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GenericHttpResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProductAsync([FromBody] ProductRequest request)
    {
        var response = await _productServices.CreateAsync(request);
        return Ok(new GenericHttpResponse<ProductResponse>
        {
            Data = response
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductByIdAsync(Guid id)
    {
        var response = await _productServices.GetByIdAsync(id);
        return Ok(new GenericHttpResponse<ProductResponse>
        {
            Data = response
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(GenericHttpResponse<IEnumerable<ProductResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProductsAsync()
    {
        var response = await _productServices.GetAllAsync();
        return Ok(new GenericHttpResponse<IEnumerable<ProductResponse>>
        {
            Data = response
        });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GenericHttpResponse<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericHttpResponse<>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProductAsync(Guid id, [FromBody] ProductRequest request)
    {
        var response = await _productServices.UpdateAsync(id, request);
        return Ok(new GenericHttpResponse<ProductResponse>
        {
            Data = response
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductAsync(Guid id)
    {
        await _productServices.DeleteByIdAsync(id);
        return NoContent();
    }
}

