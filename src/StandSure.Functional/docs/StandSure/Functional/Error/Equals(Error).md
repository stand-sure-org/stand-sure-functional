#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Error](index.md 'StandSure\.Functional\.Error')

## Error\.Equals\(Error\) Method

Compares two errors by their contents\.

```csharp
public bool Equals(StandSure.Functional.Error? other);
```
#### Parameters

<a name='StandSure.Functional.Error.Equals(StandSure.Functional.Error).other'></a>

`other` [Error](index.md 'StandSure\.Functional\.Error')

The error to compare with\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') when both describe the same failure\.

### Remarks
[Kind](Kind.md 'StandSure\.Functional\.Error\.Kind'), [Code](Code.md 'StandSure\.Functional\.Error\.Code') and [Message](Message.md 'StandSure\.Functional\.Error\.Message') \(ordinal\); [Metadata](Metadata.md 'StandSure\.Functional\.Error\.Metadata') as a set of
            key/value pairs, in any order; [Inner](Inner.md 'StandSure\.Functional\.Error\.Inner') recursively; [Errors](Errors.md 'StandSure\.Functional\.Error\.Errors') in order; and
            [Exception](Exception.md 'StandSure\.Functional\.Error\.Exception') by reference\.