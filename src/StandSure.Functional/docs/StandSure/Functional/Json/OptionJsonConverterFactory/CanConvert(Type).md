#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json').[OptionJsonConverterFactory](index.md 'StandSure\.Functional\.Json\.OptionJsonConverterFactory')

## OptionJsonConverterFactory\.CanConvert\(Type\) Method

When overridden in a derived class, determines whether the converter instance can convert the specified object type\.

```csharp
public override bool CanConvert(System.Type typeToConvert);
```
#### Parameters

<a name='StandSure.Functional.Json.OptionJsonConverterFactory.CanConvert(System.Type).typeToConvert'></a>

`typeToConvert` [System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')

The type of the object to check whether it can be converted by this converter instance\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') if the instance can convert the specified object type; otherwise, [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool')\.