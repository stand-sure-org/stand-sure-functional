#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Error](index.md 'StandSure\.Functional\.Error')

## Error\.Is\(string\) Method

Determines whether this error or any error in its [Inner](Inner.md 'StandSure\.Functional\.Error\.Inner') chain has the given code\.

```csharp
public bool Is(string code);
```
#### Parameters

<a name='StandSure.Functional.Error.Is(string).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The code to look for\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') if found\.