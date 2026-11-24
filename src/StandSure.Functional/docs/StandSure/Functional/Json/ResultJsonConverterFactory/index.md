#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json')

## ResultJsonConverterFactory Class

Creates converters for [Result&lt;T&gt;](../../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\.

```csharp
public sealed class ResultJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [System\.Text\.Json\.Serialization\.JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter 'System\.Text\.Json\.Serialization\.JsonConverter') → [System\.Text\.Json\.Serialization\.JsonConverterFactory](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverterfactory 'System\.Text\.Json\.Serialization\.JsonConverterFactory') → ResultJsonConverterFactory

### Remarks
Wire format: `{"value":T}` or `{"error":{...}}`\. Reading: a non\-null `error` wins; otherwise a
present `value` is a success \(a `null` value is read by the caller's options for `T`, so it throws
[System\.Text\.Json\.JsonException](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonexception 'System\.Text\.Json\.JsonException') for a non\-nullable value type and is a success holding `null` for a nullable or
reference type; nullable annotations on `T` are not enforced\); an empty object, or
`value` and `error` both null, is the [Uninitialized](../../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') failure\. Unknown properties are
skipped\. A JSON token that is not an object throws [System\.Text\.Json\.JsonException](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonexception 'System\.Text\.Json\.JsonException'), as for any converter\.

| Methods | |
| :--- | :--- |
| [CanConvert\(Type\)](CanConvert(Type).md 'StandSure\.Functional\.Json\.ResultJsonConverterFactory\.CanConvert\(System\.Type\)') | When overridden in a derived class, determines whether the converter instance can convert the specified object type\. |
| [CreateConverter\(Type, JsonSerializerOptions\)](CreateConverter(Type,JsonSerializerOptions).md 'StandSure\.Functional\.Json\.ResultJsonConverterFactory\.CreateConverter\(System\.Type, System\.Text\.Json\.JsonSerializerOptions\)') | Creates a converter for a specified type\. |
