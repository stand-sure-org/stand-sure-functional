#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.Map\<TOut\>\(Func\<T,TOut\>\) Method

Transforms the value; none passes through unchanged\.

```csharp
public StandSure.Functional.Option<TOut> Map<TOut>(System.Func<T,TOut> map);
```
#### Type parameters

<a name='StandSure.Functional.Option_T_.Map_TOut_(System.Func_T,TOut_).TOut'></a>

`TOut`

The new value type\.
#### Parameters

<a name='StandSure.Functional.Option_T_.Map_TOut_(System.Func_T,TOut_).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](Map_TOut_(Func_T,TOut_).md#StandSure.Functional.Option_T_.Map_TOut_(System.Func_T,TOut_).TOut 'StandSure\.Functional\.Option\<T\>\.Map\<TOut\>\(System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The transformation\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns some into none and records a
            `result.null_delegate` failure\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') return also gives none\.

#### Returns
[StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[TOut](Map_TOut_(Func_T,TOut_).md#StandSure.Functional.Option_T_.Map_TOut_(System.Func_T,TOut_).TOut 'StandSure\.Functional\.Option\<T\>\.Map\<TOut\>\(System\.Func\<T,TOut\>\)\.TOut')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')  
Some holding the mapped value, or none\.