#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## Unit Struct

The type with exactly one value, used where a result carries no information
\(the functional equivalent of [void](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/void 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/void')\)\.

```csharp
public readonly struct Unit : System.IEquatable<StandSure.Functional.Unit>, System.IComparable<StandSure.Functional.Unit>
```

Implements [System\.IEquatable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1')[Unit](index.md 'StandSure\.Functional\.Unit')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1'), [System\.IComparable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1 'System\.IComparable\`1')[Unit](index.md 'StandSure\.Functional\.Unit')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.icomparable-1 'System\.IComparable\`1')

### Remarks
Inspired by the `Unit` type in language\-ext by Paul Louth \(MIT licensed\)\.

| Fields | |
| :--- | :--- |
| [Value](Value.md 'StandSure\.Functional\.Unit\.Value') | The single [Unit](index.md 'StandSure\.Functional\.Unit') value\. |

| Methods | |
| :--- | :--- |
| [CompareTo\(Unit\)](CompareTo(Unit).md 'StandSure\.Functional\.Unit\.CompareTo\(StandSure\.Functional\.Unit\)') | Always `0`: every [Unit](index.md 'StandSure\.Functional\.Unit') compares equal\. |
| [Equals\(object\)](Equals.md#StandSure.Functional.Unit.Equals(object) 'StandSure\.Functional\.Unit\.Equals\(object\)') | Indicates whether this instance and a specified object are equal\. |
| [Equals\(Unit\)](Equals.md#StandSure.Functional.Unit.Equals(StandSure.Functional.Unit) 'StandSure\.Functional\.Unit\.Equals\(StandSure\.Functional\.Unit\)') | Always [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool'): every [Unit](index.md 'StandSure\.Functional\.Unit') is equal\. |
| [GetHashCode\(\)](GetHashCode().md 'StandSure\.Functional\.Unit\.GetHashCode\(\)') | Returns the hash code for this instance\. |
| [ToString\(\)](ToString().md 'StandSure\.Functional\.Unit\.ToString\(\)') | Returns the fully qualified type name of this instance\. |

| Operators | |
| :--- | :--- |
| [operator ==\(Unit, Unit\)](operator(Unit,Unit).md 'StandSure\.Functional\.Unit\.op\_Equality\(StandSure\.Functional\.Unit, StandSure\.Functional\.Unit\)') | Always [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool')\. |
| [operator \!=\(Unit, Unit\)](operator!(Unit,Unit).md 'StandSure\.Functional\.Unit\.op\_Inequality\(StandSure\.Functional\.Unit, StandSure\.Functional\.Unit\)') | Always [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool')\. |
