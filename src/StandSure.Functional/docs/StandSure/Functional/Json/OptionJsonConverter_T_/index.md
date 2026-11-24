#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json')

## OptionJsonConverter\<T\> Class

JSON converter for [Option&lt;T&gt;](../../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')\.

```csharp
public sealed class OptionJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<StandSure.Functional.Option<T>>
```
#### Type parameters

<a name='StandSure.Functional.Json.OptionJsonConverter_T_.T'></a>

`T`

The option value type\.

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [System\.Text\.Json\.Serialization\.JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter 'System\.Text\.Json\.Serialization\.JsonConverter') → [System\.Text\.Json\.Serialization\.JsonConverter&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1 'System\.Text\.Json\.Serialization\.JsonConverter\`1')[StandSure\.Functional\.Option&lt;](../../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')[T](index.md#StandSure.Functional.Json.OptionJsonConverter_T_.T 'StandSure\.Functional\.Json\.OptionJsonConverter\<T\>\.T')[&gt;](../../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1 'System\.Text\.Json\.Serialization\.JsonConverter\`1') → OptionJsonConverter\<T\>

### Remarks

Wire format is described on [OptionJsonConverterFactory](../OptionJsonConverterFactory/index.md 'StandSure\.Functional\.Json\.OptionJsonConverterFactory').

Native AOT: the factory builds this type by reflection, which cannot work for a value-type [T](index.md#StandSure.Functional.Json.OptionJsonConverter_T_.T 'StandSure\.Functional\.Json\.OptionJsonConverter\<T\>\.T')
the compiler never instantiated. Register it explicitly instead, with
`[JsonSourceGenerationOptions(Converters = [typeof(OptionJsonConverter<MyStruct>)])]` on your context,
or `options.Converters.Add(new OptionJsonConverter<MyStruct>())`, and add `Option<MyStruct>`
and `MyStruct` to your context with `[JsonSerializable]`.

| Methods | |
| :--- | :--- |
| [Read\(Utf8JsonReader, Type, JsonSerializerOptions\)](Read(Utf8JsonReader,Type,JsonSerializerOptions).md 'StandSure\.Functional\.Json\.OptionJsonConverter\<T\>\.Read\(System\.Text\.Json\.Utf8JsonReader, System\.Type, System\.Text\.Json\.JsonSerializerOptions\)') | Reads and converts the JSON to type [T](index.md#StandSure.Functional.Json.OptionJsonConverter_T_.T 'StandSure\.Functional\.Json\.OptionJsonConverter\<T\>\.T')\. |
