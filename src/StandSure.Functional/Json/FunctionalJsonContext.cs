using System.Text.Json.Serialization;

namespace StandSure.Functional.Json;

/// <summary>Source-generated metadata for the library's own wire types (<see cref="Error"/>, <see cref="ErrorKind"/>).</summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(ErrorKind))]
internal sealed partial class FunctionalJsonContext : JsonSerializerContext
{
}
