#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultTaskExtensions](index.md 'StandSure\.Functional\.ResultTaskExtensions')

## ResultTaskExtensions\.MapAsync Method

| Overloads | |
| :--- | :--- |
| [MapAsync&lt;T,TOut&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,Task&lt;TOut&gt;&gt;\)](MapAsync.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__) 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)') | Transforms the success value of an awaited result with an asynchronous function\. |
| [MapAsync&lt;T,TOut&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,TOut&gt;\)](MapAsync.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_) 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,TOut\>\)') | Transforms the success value of an awaited result\. |

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__)'></a>

## ResultTaskExtensions\.MapAsync\<T,TOut\>\(this Task\<Result\<T\>\>, Func\<T,Task\<TOut\>\>\) Method

Transforms the success value of an awaited result with an asynchronous function\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<TOut>> MapAsync<T,TOut>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,System.Threading.Tasks.Task<TOut>?> map);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).T'></a>

`T`

The input value type\.

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).TOut'></a>

`TOut`

The output value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).T 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).T 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The asynchronous transformation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_TOut__).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<TOut\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The mapped result, or the original failure\.

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_)'></a>

## ResultTaskExtensions\.MapAsync\<T,TOut\>\(this Task\<Result\<T\>\>, Func\<T,TOut\>\) Method

Transforms the success value of an awaited result\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<TOut>> MapAsync<T,TOut>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,TOut> map);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).T'></a>

`T`

The input value type\.

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).TOut'></a>

`TOut`

The output value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).T 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,TOut\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).T 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,TOut\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The transformation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.MapAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,TOut_).TOut 'StandSure\.Functional\.ResultTaskExtensions\.MapAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,TOut\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The mapped result, or the original failure\.