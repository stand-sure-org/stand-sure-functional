#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.Filter\(Func\<T,bool\>\) Method

Keeps some only when the predicate holds\.

```csharp
public StandSure.Functional.Option<T> Filter(System.Func<T,bool> predicate);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.Filter(System.Func_T,bool_).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The check on the value\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns some into none and
            records a `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')  
This option if it is some and the predicate holds; otherwise none\.