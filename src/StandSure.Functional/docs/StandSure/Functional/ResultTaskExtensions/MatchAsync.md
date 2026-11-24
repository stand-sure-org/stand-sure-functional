#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultTaskExtensions](index.md 'StandSure\.Functional\.ResultTaskExtensions')

## ResultTaskExtensions\.MatchAsync Method

| Overloads | |
| :--- | :--- |
| [MatchAsync&lt;T,TOut&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;Error,Task&lt;TOut&gt;&gt;, Func&lt;T,Task&lt;TOut&gt;&gt;\)](MatchAsync.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__) 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<TOut\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)') | Runs one of two asynchronous functions on an awaited result\. The failure function comes first\. |
| [MatchAsync&lt;T,TOut&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;Error,TOut&gt;, Func&lt;T,TOut&gt;\)](MatchAsync.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_) 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)') | Runs one of two functions on an awaited result\. The failure function comes first\. |

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__)'></a>

## ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this Task\<Result\<T\>\>, Func\<Error,Task\<TOut\>\>, Func\<T,Task\<TOut\>\>\) Method

Runs one of two asynchronous functions on an awaited result\. The failure function comes first\.

```csharp
public static System.Threading.Tasks.Task<TOut> MatchAsync<T,TOut>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<StandSure.Functional.Error,System.Threading.Tasks.Task<TOut>> onFailure, System.Func<T,System.Threading.Tasks.Task<TOut>?> onSuccess);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).T'></a>

`T`

The value type\.

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).TOut'></a>

`TOut`

The type both functions return\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).T 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<TOut\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).onFailure'></a>

`onFailure` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<TOut\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs on a failure, including `default`\.

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).onSuccess'></a>

`onSuccess` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).T 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<TOut\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<TOut\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs on a success\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_TOut__,System.Func_T,System.Threading.Tasks.Task_TOut__).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<TOut\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the function that ran, or `default(TOut)` when that function or the task it
            returned is [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\.

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_)'></a>

## ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this Task\<Result\<T\>\>, Func\<Error,TOut\>, Func\<T,TOut\>\) Method

Runs one of two functions on an awaited result\. The failure function comes first\.

```csharp
public static System.Threading.Tasks.Task<TOut> MatchAsync<T,TOut>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<StandSure.Functional.Error,TOut> onFailure, System.Func<T,TOut> onSuccess);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).T'></a>

`T`

The value type\.

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut'></a>

`TOut`

The type both functions return\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).T 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).onFailure'></a>

`onFailure` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs on a failure, including `default`\.

<a name='StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).onSuccess'></a>

`onSuccess` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).T 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs on a success\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MatchAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,TOut_,System.Func_T,TOut_).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MatchAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the function that ran, or `default(TOut)` when that function is [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\.