using Lab3.Contracts.Services.Order;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Exceptions;

namespace Presentation.Grpc.Validators;

internal sealed class AddOrderItemRequestValidator : IRequestValidator<AddOrderItemRequest>
{
    public void Validate(AddOrderItemRequest request)
    {
        var errors = new List<string>();

        if (request.OrderId < 0)
        {
            errors.Add("OrderId must be greater than 0");
        }

        if (request.ProductId < 0)
        {
            errors.Add("ProductId must be greater than 0");
        }

        if (request.Quantity < 0)
        {
            errors.Add("Quantity must be greater than 0");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}