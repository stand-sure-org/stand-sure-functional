#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Error](index.md 'StandSure\.Functional\.Error')

## Error\.Code Property

Gets the stable machine\-readable code\.

```csharp
public string Code { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

### Remarks
Keep codes constant, such as `user.not_found`\. The code is the `error.code` tag on the
`functional.failures` counter, so a code built from an id \(`$"user.{id}.not_found"`\) makes a new metric
series per id\. Put ids and values in [Metadata](Metadata.md 'StandSure\.Functional\.Error\.Metadata') instead\.