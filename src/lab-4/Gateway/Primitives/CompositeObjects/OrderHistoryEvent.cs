using System.Text.Json.Serialization;

namespace Gateway.Primitives.CompositeObjects;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ItemAdded), typeDiscriminator: nameof(ItemAdded))]
[JsonDerivedType(typeof(ItemRemoved), typeDiscriminator: nameof(ItemRemoved))]
[JsonDerivedType(typeof(OrderCreated), typeDiscriminator: nameof(OrderCreated))]
[JsonDerivedType(typeof(StateChanged), typeDiscriminator: nameof(StateChanged))]
[JsonDerivedType(typeof(ProcessingStageChanged), typeDiscriminator: nameof(ProcessingStageChanged))]
public record OrderHistoryEvent;
