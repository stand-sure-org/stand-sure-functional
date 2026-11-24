#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.Match\<TOut\>\(Func\<TOut\>, Func\<T,TOut\>\) Method

Runs one of two functions depending on the case\. The none function comes first\.

```csharp
public TOut Match<TOut>(System.Func<TOut> onNone, System.Func<T,TOut> onSome);
```
#### Type parameters

<a name='StandSure.Functional.Option_T_.Match_TOut_(System.Func_TOut_,System.Func_T,TOut_).TOut'></a>

`TOut`

The type both functions return\.
#### Parameters

<a name='StandSure.Functional.Option_T_.Match_TOut_(System.Func_TOut_,System.Func_T,TOut_).onNone'></a>

`onNone` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[TOut](Match_TOut_(Func_TOut_,Func_T,TOut_).md#StandSure.Functional.Option_T_.Match_TOut_(System.Func_TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.Option\<T\>\.Match\<TOut\>\(System\.Func\<TOut\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

Runs on none, including `default`\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value makes none return `default(TOut)`\.

<a name='StandSure.Functional.Option_T_.Match_TOut_(System.Func_TOut_,System.Func_T,TOut_).onSome'></a>

`onSome` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](Match_TOut_(Func_TOut_,Func_T,TOut_).md#StandSure.Functional.Option_T_.Match_TOut_(System.Func_TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.Option\<T\>\.Match\<TOut\>\(System\.Func\<TOut\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs on some with the value\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value makes some return `default(TOut)`\.

#### Returns
[TOut](Match_TOut_(Func_TOut_,Func_T,TOut_).md#StandSure.Functional.Option_T_.Match_TOut_(System.Func_TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.Option\<T\>\.Match\<TOut\>\(System\.Func\<TOut\>, System\.Func\<T,TOut\>\)\.TOut')  
The result of the function that ran\.