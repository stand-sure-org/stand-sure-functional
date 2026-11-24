#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Error](index.md 'StandSure\.Functional\.Error')

## Error\.Wrap\(string, string\) Method

Wraps this error in a new one of the same [Kind](Kind.md 'StandSure\.Functional\.Error\.Kind')\.

```csharp
public StandSure.Functional.Error Wrap(string code, string message);
```
#### Parameters

<a name='StandSure.Functional.Error.Wrap(string,string).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The new error code\.

<a name='StandSure.Functional.Error.Wrap(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The new message\.

#### Returns
[Error](index.md 'StandSure\.Functional\.Error')  
A new error whose [Inner](Inner.md 'StandSure\.Functional\.Error\.Inner') is this error\.