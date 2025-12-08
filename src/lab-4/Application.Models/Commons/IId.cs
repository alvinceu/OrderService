namespace Application.Models.Commons;

public interface IId<TSelf> where TSelf : IId<TSelf>, IEquatable<TSelf>
{
    long Value { get; }

    static abstract TSelf? Create(long value);
}