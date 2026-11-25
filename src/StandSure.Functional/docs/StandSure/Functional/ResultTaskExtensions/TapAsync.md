#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultTaskExtensions](index.md 'StandSure\.Functional\.ResultTaskExtensions')

## ResultTaskExtensions\.TapAsync Method

| Overloads | |
| :--- | :--- |
| [TapAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Action&lt;T&gt;\)](TapAsync.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_) 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Action\<T\>\)') | Runs a side effect on the value of an awaited success and returns the result unchanged\. |
| [TapAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,Task&gt;\)](TapAsync.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_) 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\>\)') | Runs an asynchronous side effect on the value of an awaited success and returns the result             unchanged\. |

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_)'></a>

## ResultTaskExtensions\.TapAsync\<T\>\(this Task\<Result\<T\>\>, Action\<T\>\) Method

Runs a side effect on the value of an awaited success and returns the result unchanged\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> TapAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Action<T> action);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Action\<T\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The task producing the result\.

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_).action'></a>

`action` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Action\<T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

The side effect\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Action_T_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Action\<T\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The awaited result\.

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_)'></a>

## ResultTaskExtensions\.TapAsync\<T\>\(this Task\<Result\<T\>\>, Func\<T,Task\>\) Method

Runs an asynchronous side effect on the value of an awaited success and returns the result
            unchanged\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> TapAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,System.Threading.Tasks.Task?> action);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The task producing the result\.

<a name='StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_).action'></a>

`action` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The side effect\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure, and a [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') task returned by it into a
            `result.null_task` failure\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.TapAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_).T 'StandSure\.Functional\.ResultTaskExtensions\.TapAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The awaited result\.