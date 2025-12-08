using Lab3.Contracts.Services.Order;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Exceptions;

namespace Presentation.Grpc.Validators;

internal sealed class SetOrderStateCancelledRequestValidator : IRequestValidator<SetOrderStateCancelledRequest>
{
    public void Validate(SetOrderStateCancelledRequest request)
    {
        if (request.OrderId < 0)
        {
            throw new ValidationException(["OrderId must be greater than 0"]);
        }
    }
}