#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json').[OptionJsonConverterFactory](index.md 'StandSure\.Functional\.Json\.OptionJsonConverterFactory')

## OptionJsonConverterFactory\.CreateConverter\(Type, JsonSerializerOptions\) Method

Creates a converter for a specified type\.

```csharp
public override System.Text.Json.Serialization.JsonConverter? CreateConverter(System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options);
```
#### Parameters

<a name='StandSure.Functional.Json.OptionJsonConverterFactory.CreateConverter(System.Type,System.Text.Json.JsonSerializerOptions).typeToConvert'></a>

`typeToConvert` [System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')

The type handled by the converter\.

<a name='StandSure.Functional.Json.OptionJsonConverterFactory.CreateConverter(System.Type,System.Text.Json.JsonSerializerOptions).options'></a>

`options` [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')

The serialization options to use\.

#### Returns
[System\.Text\.Json\.Serialization\.JsonConverter](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonconverter 'System\.Text\.Json\.Serialization\.JsonConverter')  
A converter for which T is compatible with [typeToConvert](CreateConverter(Type,JsonSerializerOptions).md#StandSure.Functional.Json.OptionJsonConverterFactory.CreateConverter(System.Type,System.Text.Json.JsonSerializerOptions).typeToConvert 'StandSure\.Functional\.Json\.OptionJsonConverterFactory\.CreateConverter\(System\.Type, System\.Text\.Json\.JsonSerializerOptions\)\.typeToConvert')\.