#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.Value Property

Gets the current case as an object: the value, or [Instance](../None/Instance.md 'StandSure\.Functional\.None\.Instance')
            when absent\. Never throws\. Boxes a value\-type [T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T'), so prefer `switch` or
            `TryGetValue`\.

```csharp
public object? Value { get; }
```

Implements [Value](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.iunion.value 'System\.Runtime\.CompilerServices\.IUnion\.Value')

#### Property Value
[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')