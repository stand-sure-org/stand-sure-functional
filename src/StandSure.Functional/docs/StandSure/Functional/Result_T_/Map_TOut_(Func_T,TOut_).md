#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Map\<TOut\>\(Func\<T,TOut\>\) Method

Transforms the success value; a failure passes through unchanged\.

```csharp
public StandSure.Functional.Result<TOut> Map<TOut>(System.Func<T,TOut> map);
```
#### Type parameters

<a name='StandSure.Functional.Result_T_.Map_TOut_(System.Func_T,TOut_).TOut'></a>

`TOut`

The new value type\.
#### Parameters

<a name='StandSure.Functional.Result_T_.Map_TOut_(System.Func_T,TOut_).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](Map_TOut_(Func_T,TOut_).md#StandSure.Functional.Result_T_.Map_TOut_(System.Func_T,TOut_).TOut 'StandSure\.Functional\.Result\<T\>\.Map\<TOut\>\(System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The transformation\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[TOut](Map_TOut_(Func_T,TOut_).md#StandSure.Functional.Result_T_.Map_TOut_(System.Func_T,TOut_).TOut 'StandSure\.Functional\.Result\<T\>\.Map\<TOut\>\(System\.Func\<T,TOut\>\)\.TOut')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')  
A success holding the mapped value, or the original failure\.