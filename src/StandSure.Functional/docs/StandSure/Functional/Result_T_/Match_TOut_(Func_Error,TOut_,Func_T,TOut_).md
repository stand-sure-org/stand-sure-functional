#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Match\<TOut\>\(Func\<Error,TOut\>, Func\<T,TOut\>\) Method

Runs one of two functions depending on the case\. The failure function comes first\.

```csharp
public TOut Match<TOut>(System.Func<StandSure.Functional.Error,TOut> onFailure, System.Func<T,TOut> onSuccess);
```
#### Type parameters

<a name='StandSure.Functional.Result_T_.Match_TOut_(System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut'></a>

`TOut`

The type both functions return\.
#### Parameters

<a name='StandSure.Functional.Result_T_.Match_TOut_(System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).onFailure'></a>

`onFailure` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](Match_TOut_(Func_Error,TOut_,Func_T,TOut_).md#StandSure.Functional.Result_T_.Match_TOut_(System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.Result\<T\>\.Match\<TOut\>\(System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs on a failure, including `default`, with the error\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value makes a failure return `default(TOut)`\.

<a name='StandSure.Functional.Result_T_.Match_TOut_(System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).onSuccess'></a>

`onSuccess` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](Match_TOut_(Func_Error,TOut_,Func_T,TOut_).md#StandSure.Functional.Result_T_.Match_TOut_(System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.Result\<T\>\.Match\<TOut\>\(System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs on a success with the value\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value makes a success return `default(TOut)`\.

#### Returns
[TOut](Match_TOut_(Func_Error,TOut_,Func_T,TOut_).md#StandSure.Functional.Result_T_.Match_TOut_(System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.Result\<T\>\.Match\<TOut\>\(System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.TOut')  
The result of the function that ran\.