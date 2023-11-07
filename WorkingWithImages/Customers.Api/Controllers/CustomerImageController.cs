using System.Net;
using Customers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Controllers;

[ApiController]
public class CustomerImageController : ControllerBase
{
    private readonly ICustomerImageService _customerImageService;

    public CustomerImageController(ICustomerImageService customerImageService)
    {
        _customerImageService = customerImageService;
    }

    [HttpPost("customers/{id:guid}/image")]
    public async Task<IActionResult> Upload([FromRoute] Guid id,
        [FromForm(Name = "Data")] IFormFile file)
    {
        var response = await _customerImageService.UploadImageAsync(id, file);

        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return Ok();
        }
        
        return BadRequest();
    }

    [HttpGet("customers/{id:guid}/image")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var response = await _customerImageService.GetImageAsync(id);

        if (response is null)
        {
            return NotFound(); 
        }
        
        return File(response.ResponseStream, response.Headers.ContentType);
    }
    
    [HttpDelete("customers/{id:guid}/image")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var response = await _customerImageService.DeleteImageAsync(id);
        return response.HttpStatusCode switch
        {
            HttpStatusCode.NoContent => Ok(),
            HttpStatusCode.NotFound => NotFound(),
            _ => BadRequest()
        };
    }
}
