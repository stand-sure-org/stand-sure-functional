#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Error](index.md 'StandSure\.Functional\.Error')

## Error\.FromException\(Exception\) Method

Creates an [Unexpected](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unexpected 'StandSure\.Functional\.ErrorKind\.Unexpected') error from an exception\.

```csharp
public static StandSure.Functional.Error FromException(System.Exception? exception);
```
#### Parameters

<a name='StandSure.Functional.Error.FromException(System.Exception).exception'></a>

`exception` [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')

The exception; kept in [Exception](Exception.md 'StandSure\.Functional\.Error\.Exception')\.

#### Returns
[Error](index.md 'StandSure\.Functional\.Error')  
The error, with code `exception.unhandled`\.