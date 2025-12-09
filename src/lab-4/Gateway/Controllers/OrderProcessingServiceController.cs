using Gateway.Clients;
using Gateway.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("api/order-processing")]
public sealed class OrderProcessingServiceController(OrderProcessingServiceClient orderProcessingServiceClient) : ControllerBase
{
    [HttpPatch("{orderId}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveAsync(
        [FromRoute] long orderId,
        [FromBody] ApproveOrderDto dto,
        CancellationToken cancellationToken)
    {
        await orderProcessingServiceClient.ApproveOrderAsync(orderId, dto, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{orderId}/packing/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartPackingAsync(
        [FromRoute] long orderId,
        [FromBody] StartOrderPackingDto dto,
        CancellationToken cancellationToken)
    {
        await orderProcessingServiceClient.StartOrderPackingAsync(orderId, dto, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{orderId}/packing/finish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FinishPackingAsync(
        [FromRoute] long orderId,
        [FromBody] FinishOrderPackingDto dto,
        CancellationToken cancellationToken)
    {
        await orderProcessingServiceClient.FinishOrderPackingAsync(orderId, dto, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{orderId}/delivery/start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartDeliveryAsync(
        [FromRoute] long orderId,
        [FromBody] StartOrderDeliveryDto dto,
        CancellationToken cancellationToken)
    {
        await orderProcessingServiceClient.StartOrderDeliveryAsync(orderId, dto, cancellationToken);

        return NoContent();
    }

    [HttpPatch("{orderId}/delivery/finish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FinishDeliveryAsync(
        [FromRoute] long orderId,
        [FromBody] FinishOrderDeliveryDto dto,
        CancellationToken cancellationToken)
    {
        await orderProcessingServiceClient.FinishOrderDeliveryAsync(orderId, dto, cancellationToken);

        return NoContent();
    }
}