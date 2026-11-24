#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Bind\<TOut\>\(Func\<T,Result\<TOut\>\>\) Method

Chains an operation that itself returns a [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>'); a failure short\-circuits\.

```csharp
public StandSure.Functional.Result<TOut> Bind<TOut>(System.Func<T,StandSure.Functional.Result<TOut>> bind);
```
#### Type parameters

<a name='StandSure.Functional.Result_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Result_TOut__).TOut'></a>

`TOut`

The value type of the chained result\.
#### Parameters

<a name='StandSure.Functional.Result_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Result_TOut__).bind'></a>

`bind` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[TOut](Bind_TOut_(Func_T,Result_TOut__).md#StandSure.Functional.Result_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.Result\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The next step\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[TOut](Bind_TOut_(Func_T,Result_TOut__).md#StandSure.Functional.Result_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.Result\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')  
The result of [bind](Bind_TOut_(Func_T,Result_TOut__).md#StandSure.Functional.Result_T_.Bind_TOut_(System.Func_T,StandSure.Functional.Result_TOut__).bind 'StandSure\.Functional\.Result\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.bind'), or the original failure without calling it\.