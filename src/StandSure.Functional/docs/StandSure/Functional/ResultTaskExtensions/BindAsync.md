#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultTaskExtensions](index.md 'StandSure\.Functional\.ResultTaskExtensions')

## ResultTaskExtensions\.BindAsync Method

| Overloads | |
| :--- | :--- |
| [BindAsync&lt;T,TOut&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,Result&lt;TOut&gt;&gt;\)](BindAsync.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__) 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)') | Chains a result\-returning step onto an awaited result\. |
| [BindAsync&lt;T,TOut&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,Task&lt;Result&lt;TOut&gt;&gt;&gt;\)](BindAsync.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___) 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<TOut\>\>\>\)') | Chains an asynchronous result\-returning step onto an awaited result\. |

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__)'></a>

## ResultTaskExtensions\.BindAsync\<T,TOut\>\(this Task\<Result\<T\>\>, Func\<T,Result\<TOut\>\>\) Method

Chains a result\-returning step onto an awaited result\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<TOut>> BindAsync<T,TOut>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,StandSure.Functional.Result<TOut>> bind);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).T'></a>

`T`

The input value type\.

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).TOut'></a>

`TOut`

The output value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).T 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).bind'></a>

`bind` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).T 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The next step\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of [bind](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,StandSure.Functional.Result_TOut__).bind 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.bind'), or the original failure without calling it\.

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___)'></a>

## ResultTaskExtensions\.BindAsync\<T,TOut\>\(this Task\<Result\<T\>\>, Func\<T,Task\<Result\<TOut\>\>\>\) Method

Chains an asynchronous result\-returning step onto an awaited result\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<TOut>> BindAsync<T,TOut>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,System.Threading.Tasks.Task<StandSure.Functional.Result<TOut>>?> bind);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).T'></a>

`T`

The input value type\.

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).TOut'></a>

`TOut`

The output value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).T 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<TOut\>\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).bind'></a>

`bind` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).T 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<TOut\>\>\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).TOut 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<TOut\>\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The next asynchronous step\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).TOut 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<TOut\>\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of [bind](index.md#StandSure.Functional.ResultTaskExtensions.BindAsync_T,TOut_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_StandSure.Functional.Result_TOut___).bind 'StandSure\.Functional\.ResultTaskExtensions\.BindAsync\<T,TOut\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<TOut\>\>\>\)\.bind'), or the original failure without calling it\.