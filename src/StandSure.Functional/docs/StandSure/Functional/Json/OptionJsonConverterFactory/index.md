#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json')

## OptionJsonConverterFactory Class

Creates converters for [Option&lt;T&gt;](../../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')\.

```csharp
public sealed class OptionJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [System\.Text\.Json\.Serialization\.JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter 'System\.Text\.Json\.Serialization\.JsonConverter') → [System\.Text\.Json\.Serialization\.JsonConverterFactory](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverterfactory 'System\.Text\.Json\.Serialization\.JsonConverterFactory') → OptionJsonConverterFactory

### Remarks
Wire format: `{"value":T}` for some, `{}` for none\. Reading: a missing or null `value` is none;
unknown properties are skipped\. A JSON token that is not an object throws [System\.Text\.Json\.JsonException](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonexception 'System\.Text\.Json\.JsonException')\.

| Methods | |
| :--- | :--- |
| [CanConvert\(Type\)](CanConvert(Type).md 'StandSure\.Functional\.Json\.OptionJsonConverterFactory\.CanConvert\(System\.Type\)') | When overridden in a derived class, determines whether the converter instance can convert the specified object type\. |
| [CreateConverter\(Type, JsonSerializerOptions\)](CreateConverter(Type,JsonSerializerOptions).md 'StandSure\.Functional\.Json\.OptionJsonConverterFactory\.CreateConverter\(System\.Type, System\.Text\.Json\.JsonSerializerOptions\)') | Creates a converter for a specified type\. |
