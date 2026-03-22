using Gateway.Clients;
using Gateway.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("api/products")]
public class ProductServiceController(ProductServiceClient productServiceClient) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductCreatedDto>> CreateProductAsync(
        [FromBody] CreateProductDto dto,
        CancellationToken ct)
    {
        ProductCreatedDto response = await productServiceClient.CreateProductAsync(dto, ct);

        return Ok(response);
    }
}