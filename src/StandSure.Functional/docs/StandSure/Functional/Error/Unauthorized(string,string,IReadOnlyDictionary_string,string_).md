#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Error](index.md 'StandSure\.Functional\.Error')

## Error\.Unauthorized\(string, string, IReadOnlyDictionary\<string,string\>\) Method

Creates a [Unauthorized](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unauthorized 'StandSure\.Functional\.ErrorKind\.Unauthorized') error\.

```csharp
public static StandSure.Functional.Error Unauthorized(string code, string message, System.Collections.Generic.IReadOnlyDictionary<string,string>? metadata=null);
```
#### Parameters

<a name='StandSure.Functional.Error.Unauthorized(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error code\.

<a name='StandSure.Functional.Error.Unauthorized(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The message\.

<a name='StandSure.Functional.Error.Unauthorized(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_).metadata'></a>

`metadata` [System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')

Optional context\.

#### Returns
[Error](index.md 'StandSure\.Functional\.Error')  
The error\.