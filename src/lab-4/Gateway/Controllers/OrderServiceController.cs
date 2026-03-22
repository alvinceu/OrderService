using Gateway.Clients;
using Gateway.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderServiceController(OrderServiceClient orderServiceClient) : ControllerBase
{
    [HttpPatch("{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> OrderChangeStatusAsync(
        [FromRoute] long orderId,
        [FromBody] OrderChangeStatusDto dto,
        CancellationToken ct)
    {
        await orderServiceClient.OrderChangeStatusAsync(orderId, dto, ct);

        return NoContent();
    }

    [HttpGet("{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderHistoryItemDto[]>> FindOrderHistoryItemsAsync(
        [FromRoute] long orderId,
        [AsParameters] Commons.PaginationInfo info,
        CancellationToken ct)
    {
        OrderHistoryItemDto[] response =
            await orderServiceClient.FindOrderHistoryItemsAsync(orderId, info, ct).ToArrayAsync(cancellationToken: ct);

        return Ok(response);
    }

    [HttpDelete("items/{orderItemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveOrderItemAsync([FromRoute] long orderItemId, CancellationToken ct)
    {
        await orderServiceClient.RemoveOrderItemAsync(orderItemId, ct);

        return NoContent();
    }

    [HttpPost("{orderId}/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderItemAddedDto>> AddOrderItemAsync([FromRoute] long orderId, [FromBody] AddOrderItemDto dto, CancellationToken ct)
    {
        OrderItemAddedDto response = await orderServiceClient.AddOrderItemAsync(orderId, dto, ct);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderCreatedDto>> CreateOrderAsync([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        OrderCreatedDto response = await orderServiceClient.CreateOrderAsync(dto, ct);

        return Ok(response);
    }
}