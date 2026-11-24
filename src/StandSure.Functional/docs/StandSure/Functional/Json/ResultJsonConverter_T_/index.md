#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json')

## ResultJsonConverter\<T\> Class

JSON converter for [Result&lt;T&gt;](../../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\.

```csharp
public sealed class ResultJsonConverter<T> : System.Text.Json.Serialization.JsonConverter<StandSure.Functional.Result<T>>
```
#### Type parameters

<a name='StandSure.Functional.Json.ResultJsonConverter_T_.T'></a>

`T`

The result value type\.

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [System\.Text\.Json\.Serialization\.JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter 'System\.Text\.Json\.Serialization\.JsonConverter') → [System\.Text\.Json\.Serialization\.JsonConverter&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1 'System\.Text\.Json\.Serialization\.JsonConverter\`1')[StandSure\.Functional\.Result&lt;](../../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Json.ResultJsonConverter_T_.T 'StandSure\.Functional\.Json\.ResultJsonConverter\<T\>\.T')[&gt;](../../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter-1 'System\.Text\.Json\.Serialization\.JsonConverter\`1') → ResultJsonConverter\<T\>

### Remarks

Wire format is described on [ResultJsonConverterFactory](../ResultJsonConverterFactory/index.md 'StandSure\.Functional\.Json\.ResultJsonConverterFactory').

Native AOT: the factory builds this type by reflection, which cannot work for a value-type [T](index.md#StandSure.Functional.Json.ResultJsonConverter_T_.T 'StandSure\.Functional\.Json\.ResultJsonConverter\<T\>\.T')
the compiler never instantiated. Register it explicitly instead, with
`[JsonSourceGenerationOptions(Converters = [typeof(ResultJsonConverter<MyStruct>)])]` on your context,
or `options.Converters.Add(new ResultJsonConverter<MyStruct>())`, and add `Result<MyStruct>`
and `MyStruct` to your context with `[JsonSerializable]`.

| Methods | |
| :--- | :--- |
| [Read\(Utf8JsonReader, Type, JsonSerializerOptions\)](Read(Utf8JsonReader,Type,JsonSerializerOptions).md 'StandSure\.Functional\.Json\.ResultJsonConverter\<T\>\.Read\(System\.Text\.Json\.Utf8JsonReader, System\.Type, System\.Text\.Json\.JsonSerializerOptions\)') | Reads and converts the JSON to type [T](index.md#StandSure.Functional.Json.ResultJsonConverter_T_.T 'StandSure\.Functional\.Json\.ResultJsonConverter\<T\>\.T')\. |
