using Lab3.Contracts.Services.Product;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Exceptions;
using Presentation.Grpc.Mappers;

namespace Presentation.Grpc.Validators;

internal class CreateProductRequestValidator : IRequestValidator<CreateProductRequest>
{
    public void Validate(CreateProductRequest request)
    {
        var errors = new List<string>();

        if (request.Price.ToDecimal() < 0)
        {
            errors.Add("Price must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("Name must not be empty");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}