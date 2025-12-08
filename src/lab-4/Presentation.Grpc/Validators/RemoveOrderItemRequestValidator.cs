using Lab3.Contracts.Services.Order;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Exceptions;

namespace Presentation.Grpc.Validators;

internal sealed class RemoveOrderItemRequestValidator : IRequestValidator<RemoveOrderItemRequest>
{
    public void Validate(RemoveOrderItemRequest request)
    {
        if (request.OrderItemId < 0)
        {
            throw new ValidationException(["OrderItemId must be greater than 0"]);
        }
    }
}