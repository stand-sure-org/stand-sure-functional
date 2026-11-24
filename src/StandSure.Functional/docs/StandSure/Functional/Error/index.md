#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## Error Class

A failure: a kind, a stable machine\-readable code, a message and optional context\.

```csharp
public sealed record Error : System.IEquatable<StandSure.Functional.Error>
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Error

Implements [System\.IEquatable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1')[Error](index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1')

| Properties | |
| :--- | :--- |
| [Code](Code.md 'StandSure\.Functional\.Error\.Code') | Gets the stable machine\-readable code\. |
| [Errors](Errors.md 'StandSure\.Functional\.Error\.Errors') | Gets child errors \(for example several validation failures\); never [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\. |
| [Exception](Exception.md 'StandSure\.Functional\.Error\.Exception') | Gets the originating exception; kept in\-process only and never serialized\. |
| [Inner](Inner.md 'StandSure\.Functional\.Error\.Inner') | Gets the error this one wraps, if any\. |
| [Kind](Kind.md 'StandSure\.Functional\.Error\.Kind') | Gets the category of the failure\. |
| [Message](Message.md 'StandSure\.Functional\.Error\.Message') | Gets the human\-readable message\. |
| [Metadata](Metadata.md 'StandSure\.Functional\.Error\.Metadata') | Gets additional context; never [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\. |
| [Uninitialized](Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') | Gets the error that is the shared placeholder for a default \(uninitialized\) result\. |

| Methods | |
| :--- | :--- |
| [Conflict\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](Conflict(string,string,IReadOnlyDictionary_string,string_).md 'StandSure\.Functional\.Error\.Conflict\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates a [Conflict](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Conflict 'StandSure\.Functional\.ErrorKind\.Conflict') error\. |
| [Equals\(Error\)](Equals(Error).md 'StandSure\.Functional\.Error\.Equals\(StandSure\.Functional\.Error\)') | Compares two errors by their contents\. |
| [Forbidden\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](Forbidden(string,string,IReadOnlyDictionary_string,string_).md 'StandSure\.Functional\.Error\.Forbidden\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates a [Forbidden](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Forbidden 'StandSure\.Functional\.ErrorKind\.Forbidden') error\. |
| [FromException\(Exception\)](FromException(Exception).md 'StandSure\.Functional\.Error\.FromException\(System\.Exception\)') | Creates an [Unexpected](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unexpected 'StandSure\.Functional\.ErrorKind\.Unexpected') error from an exception\. |
| [GetHashCode\(\)](GetHashCode().md 'StandSure\.Functional\.Error\.GetHashCode\(\)') | Gets a hash code that agrees with [Equals\(Error\)](Equals(Error).md 'StandSure\.Functional\.Error\.Equals\(StandSure\.Functional\.Error\)')\. |
| [Is\(string\)](Is(string).md 'StandSure\.Functional\.Error\.Is\(string\)') | Determines whether this error or any error in its [Inner](Inner.md 'StandSure\.Functional\.Error\.Inner') chain has the given code\. |
| [NotFound\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](NotFound(string,string,IReadOnlyDictionary_string,string_).md 'StandSure\.Functional\.Error\.NotFound\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates a [NotFound](../ErrorKind/index.md#StandSure.Functional.ErrorKind.NotFound 'StandSure\.Functional\.ErrorKind\.NotFound') error\. |
| [Unauthorized\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](Unauthorized(string,string,IReadOnlyDictionary_string,string_).md 'StandSure\.Functional\.Error\.Unauthorized\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates a [Unauthorized](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unauthorized 'StandSure\.Functional\.ErrorKind\.Unauthorized') error\. |
| [Unavailable\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](Unavailable(string,string,IReadOnlyDictionary_string,string_).md 'StandSure\.Functional\.Error\.Unavailable\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates a [Unavailable](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unavailable 'StandSure\.Functional\.ErrorKind\.Unavailable') error\. |
| [Unexpected\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](Unexpected(string,string,IReadOnlyDictionary_string,string_).md 'StandSure\.Functional\.Error\.Unexpected\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates an [Unexpected](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unexpected 'StandSure\.Functional\.ErrorKind\.Unexpected') error\. |
| [Validation\(string, string, IEnumerable&lt;Error&gt;\)](Validation.md#StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IEnumerable_StandSure.Functional.Error_) 'StandSure\.Functional\.Error\.Validation\(string, string, System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Error\>\)') | Creates a [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation') parent error that holds several child errors\. |
| [Validation\(string, string, IReadOnlyDictionary&lt;string,string&gt;\)](Validation.md#StandSure.Functional.Error.Validation(string,string,System.Collections.Generic.IReadOnlyDictionary_string,string_) 'StandSure\.Functional\.Error\.Validation\(string, string, System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>\)') | Creates a [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation') error\. |
| [Wrap\(string, string\)](Wrap(string,string).md 'StandSure\.Functional\.Error\.Wrap\(string, string\)') | Wraps this error in a new one of the same [Kind](Kind.md 'StandSure\.Functional\.Error\.Kind')\. |
