#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultTaskExtensions](index.md 'StandSure\.Functional\.ResultTaskExtensions')

## ResultTaskExtensions\.TapErrorAsync Method

| Overloads | |
| :--- | :--- |
| [TapErrorAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Action&lt;Error&gt;\)](TapErrorAsync.md#StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_StandSure.Functional.Error_) 'StandSure\.Functional\.ResultTaskExtensions\.TapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Action\<StandSure\.Functional\.Error\>\)') | Runs a side effect on the error of an awaited failure and returns the result unchanged\. |
| [TapErrorAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;Error,Task&gt;\)](TapErrorAsync.md#StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_) 'StandSure\.Functional\.ResultTaskExtensions\.TapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\>\)') | Runs an asynchronous side effect on the error of an awaited failure \(for example a rollback\) and             returns the result unchanged\. |

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_StandSure.Functional.Error_)'></a>

## ResultTaskExtensions\.TapErrorAsync\<T\>\(this Task\<Result\<T\>\>, Action\<Error\>\) Method

Runs a side effect on the error of an awaited failure and returns the result unchanged\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> TapErrorAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Action<StandSure.Functional.Error> action);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_StandSure.Functional.Error_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_StandSure.Functional.Error_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Action\<StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The task producing the result\.

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_StandSure.Functional.Error_).action'></a>

`action` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

The side effect\. It receives [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') for `default`\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value keeps the failure unchanged and records a `result.null_delegate` failure
            in telemetry\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Action\<StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The awaited result\.

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_)'></a>

## ResultTaskExtensions\.TapErrorAsync\<T\>\(this Task\<Result\<T\>\>, Func\<Error,Task\>\) Method

Runs an asynchronous side effect on the error of an awaited failure \(for example a rollback\) and
            returns the result unchanged\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> TapErrorAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<StandSure.Functional.Error,System.Threading.Tasks.Task?> action);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The task producing the result\.

<a name='StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_).action'></a>

`action` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The side effect\. It receives [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') for `default`\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value, or a [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') task returned by it, keeps the failure unchanged
            and records the problem \(`result.null_delegate` or `result.null_task`\) in telemetry\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The awaited result\.