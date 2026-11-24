#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Try](index.md 'StandSure\.Functional\.Try')

## Try\.Run Method

| Overloads | |
| :--- | :--- |
| [Run\(Action\)](Run.md#StandSure.Functional.Try.Run(System.Action) 'StandSure\.Functional\.Try\.Run\(System\.Action\)') | Runs an action and captures a thrown exception as a failure\. |
| [Run\(Func&lt;Task&gt;\)](Run.md#StandSure.Functional.Try.Run(System.Func_System.Threading.Tasks.Task_) 'StandSure\.Functional\.Try\.Run\(System\.Func\<System\.Threading\.Tasks\.Task\>\)') | Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work             ran or failed\. Calling it does not compile; use [RunAsync\(Func&lt;CancellationToken,Task&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\>, System\.Threading\.CancellationToken\)')\. |
| [Run&lt;T&gt;\(Func&lt;Task&lt;T&gt;&gt;\)](Run.md#StandSure.Functional.Try.Run_T_(System.Func_System.Threading.Tasks.Task_T__) 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<System\.Threading\.Tasks\.Task\<T\>\>\)') | Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work             ran or failed\. Calling it does not compile; use [RunAsync&lt;T&gt;\(Func&lt;CancellationToken,Task&lt;T&gt;&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)')\. |
| [Run&lt;T&gt;\(Func&lt;T&gt;\)](Run.md#StandSure.Functional.Try.Run_T_(System.Func_T_) 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<T\>\)') | Runs a function and captures a thrown exception as a failure\. |

<a name='StandSure.Functional.Try.Run(System.Action)'></a>

## Try\.Run\(Action\) Method

Runs an action and captures a thrown exception as a failure\.

```csharp
public static StandSure.Functional.Result<StandSure.Functional.Unit> Run(System.Action action);
```
#### Parameters

<a name='StandSure.Functional.Try.Run(System.Action).action'></a>

`action` [System\.Action](https://learn.microsoft.com/en-us/dotnet/api/system.action 'System\.Action')

The action to run\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value returns a `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A success holding [Value](../Unit/Value.md 'StandSure\.Functional\.Unit\.Value'), or the captured exception as a failure\.

<a name='StandSure.Functional.Try.Run(System.Func_System.Threading.Tasks.Task_)'></a>

## Try\.Run\(Func\<Task\>\) Method

Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work
            ran or failed\. Calling it does not compile; use [RunAsync\(Func&lt;CancellationToken,Task&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\>, System\.Threading\.CancellationToken\)')\.

```csharp
public static StandSure.Functional.Result<StandSure.Functional.Unit> Run(System.Func<System.Threading.Tasks.Task> fn);
```
#### Parameters

<a name='StandSure.Functional.Try.Run(System.Func_System.Threading.Tasks.Task_).fn'></a>

`fn` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

Never invoked\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A `try.async_delegate` failure \(reachable only through reflection\)\.

<a name='StandSure.Functional.Try.Run_T_(System.Func_System.Threading.Tasks.Task_T__)'></a>

## Try\.Run\<T\>\(Func\<Task\<T\>\>\) Method

Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work
            ran or failed\. Calling it does not compile; use [RunAsync&lt;T&gt;\(Func&lt;CancellationToken,Task&lt;T&gt;&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)')\.

```csharp
public static StandSure.Functional.Result<T> Run<T>(System.Func<System.Threading.Tasks.Task<T>> fn);
```
#### Type parameters

<a name='StandSure.Functional.Try.Run_T_(System.Func_System.Threading.Tasks.Task_T__).T'></a>

`T`

The task's result type\.
#### Parameters

<a name='StandSure.Functional.Try.Run_T_(System.Func_System.Threading.Tasks.Task_T__).fn'></a>

`fn` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[T](index.md#StandSure.Functional.Try.Run_T_(System.Func_System.Threading.Tasks.Task_T__).T 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<System\.Threading\.Tasks\.Task\<T\>\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

Never invoked\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Try.Run_T_(System.Func_System.Threading.Tasks.Task_T__).T 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<System\.Threading\.Tasks\.Task\<T\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A `try.async_delegate` failure \(reachable only through reflection\)\.

<a name='StandSure.Functional.Try.Run_T_(System.Func_T_)'></a>

## Try\.Run\<T\>\(Func\<T\>\) Method

Runs a function and captures a thrown exception as a failure\.

```csharp
public static StandSure.Functional.Result<T?> Run<T>(System.Func<T?> fn);
```
#### Type parameters

<a name='StandSure.Functional.Try.Run_T_(System.Func_T_).T'></a>

`T`

The result type\.
#### Parameters

<a name='StandSure.Functional.Try.Run_T_(System.Func_T_).fn'></a>

`fn` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[T](index.md#StandSure.Functional.Try.Run_T_(System.Func_T_).T 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

The function to run\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value returns a `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Try.Run_T_(System.Func_T_).T 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<T\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
The function's value as a success, or the captured exception as a failure\.