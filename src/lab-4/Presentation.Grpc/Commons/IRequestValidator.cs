namespace Presentation.Grpc.Commons;

public interface IRequestValidator<T>
{
    void Validate(T request);
}