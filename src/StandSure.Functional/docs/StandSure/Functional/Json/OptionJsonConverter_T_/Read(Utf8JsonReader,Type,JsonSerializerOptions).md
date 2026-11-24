#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json').[OptionJsonConverter&lt;T&gt;](index.md 'StandSure\.Functional\.Json\.OptionJsonConverter\<T\>')

## OptionJsonConverter\<T\>\.Read\(Utf8JsonReader, Type, JsonSerializerOptions\) Method

Reads and converts the JSON to type [T](index.md#StandSure.Functional.Json.OptionJsonConverter_T_.T 'StandSure\.Functional\.Json\.OptionJsonConverter\<T\>\.T')\.

```csharp
public override StandSure.Functional.Option<T> Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options);
```
#### Parameters

<a name='StandSure.Functional.Json.OptionJsonConverter_T_.Read(System.Text.Json.Utf8JsonReader,System.Type,System.Text.Json.JsonSerializerOptions).reader'></a>

`reader` [System\.Text\.Json\.Utf8JsonReader](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.utf8jsonreader 'System\.Text\.Json\.Utf8JsonReader')

The reader\.

<a name='StandSure.Functional.Json.OptionJsonConverter_T_.Read(System.Text.Json.Utf8JsonReader,System.Type,System.Text.Json.JsonSerializerOptions).typeToConvert'></a>

`typeToConvert` [System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')

The type to convert\.

<a name='StandSure.Functional.Json.OptionJsonConverter_T_.Read(System.Text.Json.Utf8JsonReader,System.Type,System.Text.Json.JsonSerializerOptions).options'></a>

`options` [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')

An object that specifies serialization options to use\.

#### Returns
[StandSure\.Functional\.Option&lt;](../../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')[T](index.md#StandSure.Functional.Json.OptionJsonConverter_T_.T 'StandSure\.Functional\.Json\.OptionJsonConverter\<T\>\.T')[&gt;](../../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')  
The converted value\.