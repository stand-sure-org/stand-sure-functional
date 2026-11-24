#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Error](index.md 'StandSure\.Functional\.Error')

## Error\.Validation Method

| Overloads | |
| :--- | :--- |
| [Validation\(string, string, IEnumerable&lt;Error&gt;\)](Validation.md#StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IEnumerable_StandSure.Functional.Error_) 'StandSure\.Functional\.Error\.Validation\(string, string, System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Error\>\)') | Creates a [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation') parent error that holds several child errors\. |
| [Validation\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](Validation.md#StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_) 'StandSure\.Functional\.Error\.Validation\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates a [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation') error\. |

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IEnumerable_StandSure.Functional.Error_)'></a>

## Error\.Validation\(string, string, IEnumerable\<Error\>\) Method

Creates a [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation') parent error that holds several child errors\.

```csharp
public static StandSure.Functional.Error Validation(string code, string message, System.Collections.Generic.IEnumerable<StandSure.Functional.Error>? errors);
```
#### Parameters

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IEnumerable_StandSure.Functional.Error_).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error code\.

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IEnumerable_StandSure.Functional.Error_).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The message\.

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IEnumerable_StandSure.Functional.Error_).errors'></a>

`errors` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[Error](index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The child errors\.

#### Returns
[Error](index.md 'StandSure\.Functional\.Error')  
The error\.

### Remarks
The children were already recorded when they were built, so a parent with at least one child is recorded as a
`functional.failure.grouped` event and is not counted in `functional.failures`\. With no children it is
an ordinary root failure\.

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_)'></a>

## Error\.Validation\(string, string, IReadOnlyDictionary\<string,string\>\) Method

Creates a [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation') error\.

```csharp
public static StandSure.Functional.Error Validation(string code, string message, System.Collections.Generic.IReadOnlyDictionary<string,string>? metadata=null);
```
#### Parameters

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error code\.

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The message\.

<a name='StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_).metadata'></a>

`metadata` [System\.Collections\.Generic\.IReadOnlyDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlydictionary-2 'System\.Collections\.Generic\.IReadOnlyDictionary\`2')

Optional context\.

#### Returns
[Error](index.md 'StandSure\.Functional\.Error')  
The error\.