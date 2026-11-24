#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Value Property

Gets the current case as an object: the success value, or the [Error](../Error/index.md 'StandSure\.Functional\.Error')\.
            Never throws\. Boxes a value\-type [T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T'), so prefer `switch` or `TryGetValue`\.

```csharp
public object? Value { get; }
```

Implements [Value](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.iunion.value 'System\.Runtime\.CompilerServices\.IUnion\.Value')

#### Property Value
[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')