using Application.Abstractions.Services;
using Application.Models.Primitives.EntityIds;
using Integration.Kafka.Commons;
using Microsoft.Extensions.Logging;
using Orders.Kafka.Contracts;

namespace Presentation.Kafka.Services;

internal sealed class KafkaMessageHandler : IKafkaMessageHandler<OrderProcessingKey, OrderProcessingValue>
{
    private readonly IOrderService _orderService;

    private readonly ILogger<KafkaMessageHandler> _logger;

    public KafkaMessageHandler(IOrderService orderService, ILogger<KafkaMessageHandler> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    public async Task HandleAsync(IReadOnlyList<KafkaMessage<OrderProcessingKey, OrderProcessingValue>> messages, CancellationToken ct)
    {
        foreach (KafkaMessage<OrderProcessingKey, OrderProcessingValue> message in messages)
        {
            if (message.Value.EventCase is OrderProcessingValue.EventOneofCase.ApprovalReceived)
            {
                var id = OrderId.Create(message.Value.ApprovalReceived.OrderId);

                if (!message.Value.ApprovalReceived.IsApproved)
                {
                    try
                    {
                        await _orderService.SetCancelledAsync(id, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
                else
                {
                    await _orderService.ReportHistoryEventApprovedAsync(id, ct);
                }
            }
            else if (message.Value.EventCase is OrderProcessingValue.EventOneofCase.DeliveryStarted)
            {
                var id = OrderId.Create(message.Value.DeliveryStarted.OrderId);

                await _orderService.ReportHistoryEventInDeliveryAsync(id, ct);
            }
            else if (message.Value.EventCase is OrderProcessingValue.EventOneofCase.PackingStarted)
            {
                var id = OrderId.Create(message.Value.PackingStarted.OrderId);

                await _orderService.ReportHistoryEventPackingAsync(id, ct);
            }
            else if (message.Value.EventCase is OrderProcessingValue.EventOneofCase.DeliveryFinished)
            {
                var id = OrderId.Create(message.Value.DeliveryFinished.OrderId);

                if (!message.Value.DeliveryFinished.IsFinishedSuccessfully)
                {
                    try
                    {
                        await _orderService.SetCancelledAsync(id, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
                else
                {
                    await _orderService.ReportHistoryEventPackingAsync(id, ct);
                    await _orderService.SetCompletedAsync(id, ct);
                }
            }
            else if (message.Value.EventCase is OrderProcessingValue.EventOneofCase.PackingFinished)
            {
                var id = OrderId.Create(message.Value.PackingFinished.OrderId);

                if (!message.Value.PackingFinished.IsFinishedSuccessfully)
                {
                    try
                    {
                        await _orderService.SetCancelledAsync(id, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
                else
                {
                    await _orderService.ReportHistoryEventPackingAsync(id, ct);
                }
            }

            message.Commit();
        }
    }
}
