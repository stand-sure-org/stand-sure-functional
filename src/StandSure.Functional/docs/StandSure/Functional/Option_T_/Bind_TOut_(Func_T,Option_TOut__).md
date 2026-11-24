#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.Bind\<TOut\>\(Func\<T,Option\<TOut\>\>\) Method

Chains an operation that itself returns an [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>'); none short\-circuits\.

```csharp
public StandSure.Functional.Option<TOut> Bind<TOut>(System.Func<T,StandSure.Functional.Option<TOut>> bind);
```
#### Type parameters

<a name='StandSure.Functional.Option_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Option_TOut__).TOut'></a>

`TOut`

The value type of the chained option\.
#### Parameters

<a name='StandSure.Functional.Option_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Option_TOut__).bind'></a>

`bind` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[TOut](Bind_TOut_(Func_T,Option_TOut__).md#StandSure.Functional.Option_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Option_TOut__).TOut 'StandSure\.Functional\.Option\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Option\<TOut\>\>\)\.TOut')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The next step\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns some into none and records a
            `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[TOut](Bind_TOut_(Func_T,Option_TOut__).md#StandSure.Functional.Option_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Option_TOut__).TOut 'StandSure\.Functional\.Option\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Option\<TOut\>\>\)\.TOut')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')  
The result of [bind](Bind_TOut_(Func_T,Option_TOut__).md#StandSure.Functional.Option_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Option_TOut__).bind 'StandSure\.Functional\.Option\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Option\<TOut\>\>\)\.bind'), or none without calling it\.