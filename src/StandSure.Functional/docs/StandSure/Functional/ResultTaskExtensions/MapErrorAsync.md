#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultTaskExtensions](index.md 'StandSure\.Functional\.ResultTaskExtensions')

## ResultTaskExtensions\.MapErrorAsync Method

| Overloads | |
| :--- | :--- |
| [MapErrorAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;Error,Error&gt;\)](MapErrorAsync.md#StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,StandSure.Functional.Error_) 'StandSure\.Functional\.ResultTaskExtensions\.MapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,StandSure\.Functional\.Error\>\)') | Transforms the error of an awaited failure; a success passes through unchanged\. |
| [MapErrorAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;Error,Task&lt;Error&gt;&gt;\)](MapErrorAsync.md#StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_StandSure.Functional.Error__) 'StandSure\.Functional\.ResultTaskExtensions\.MapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Error\>\>\)') | Transforms the error of an awaited failure with an asynchronous function\. |

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,StandSure.Functional.Error_)'></a>

## ResultTaskExtensions\.MapErrorAsync\<T\>\(this Task\<Result\<T\>\>, Func\<Error,Error\>\) Method

Transforms the error of an awaited failure; a success passes through unchanged\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> MapErrorAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<StandSure.Functional.Error,StandSure.Functional.Error> map);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,StandSure.Functional.Error_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,StandSure.Functional.Error_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.MapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The task producing the result\.

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,StandSure.Functional.Error_).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The transformation\. It receives [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') for `default`\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') return becomes [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') delegate
            keeps the original failure and records a `result.null_delegate` failure in telemetry\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.MapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The original success, or a failure with the mapped error\.

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_StandSure.Functional.Error__)'></a>

## ResultTaskExtensions\.MapErrorAsync\<T\>\(this Task\<Result\<T\>\>, Func\<Error,Task\<Error\>\>\) Method

Transforms the error of an awaited failure with an asynchronous function\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> MapErrorAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<StandSure.Functional.Error,System.Threading.Tasks.Task<StandSure.Functional.Error>?> map);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_StandSure.Functional.Error__).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_StandSure.Functional.Error__).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_StandSure.Functional.Error__).T 'StandSure\.Functional\.ResultTaskExtensions\.MapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Error\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The task producing the result\.

<a name='StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_StandSure.Functional.Error__).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The transformation\. It receives [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') for `default`\.
            A task that yields [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') gives [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')
            delegate, or a [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') task returned by it, keeps the original failure and records the problem
            \(`result.null_delegate` or `result.null_task`\) in telemetry\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapErrorAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_StandSure.Functional.Error,System.Threading.Tasks.Task_StandSure.Functional.Error__).T 'StandSure\.Functional\.ResultTaskExtensions\.MapErrorAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<StandSure\.Functional\.Error,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Error\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The original success, or a failure with the mapped error\.