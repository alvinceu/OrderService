using Lab3.Contracts.Services.Order;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Exceptions;

namespace Presentation.Grpc.Validators;

internal sealed class FindOrderHistoryItemsRequestValidator : IRequestValidator<FindOrderHistoryItemsRequest>
{
    public void Validate(FindOrderHistoryItemsRequest request)
    {
        var errors = new List<string>();

        if (request.OrderId < 0)
        {
            errors.Add("OrderId must be greater than 0");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}