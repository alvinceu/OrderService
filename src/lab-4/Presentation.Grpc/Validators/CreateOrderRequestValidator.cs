using Lab3.Contracts.Services.Order;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Exceptions;

namespace Presentation.Grpc.Validators;

internal sealed class CreateOrderRequestValidator : IRequestValidator<CreateOrderRequest>
{
    public void Validate(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CreatedBy))
        {
            throw new ValidationException(["CreatedBy must not be empty"]);
        }
    }
}