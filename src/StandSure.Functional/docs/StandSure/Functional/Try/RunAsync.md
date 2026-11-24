#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Try](index.md 'StandSure\.Functional\.Try')

## Try\.RunAsync Method

| Overloads | |
| :--- | :--- |
| [RunAsync\(Func&lt;CancellationToken,Task&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\>, System\.Threading\.CancellationToken\)') | Runs an asynchronous function that returns no value and captures a thrown exception as a failure\. |
| [RunAsync&lt;T&gt;\(Func&lt;CancellationToken,Task&lt;T&gt;&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)') | Runs an asynchronous function and captures a thrown exception as a failure\. |

<a name='StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken)'></a>

## Try\.RunAsync\(Func\<CancellationToken,Task\>, CancellationToken\) Method

Runs an asynchronous function that returns no value and captures a thrown exception as a failure\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<StandSure.Functional.Unit>> RunAsync(System.Func<System.Threading.CancellationToken,System.Threading.Tasks.Task?> fn, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken).fn'></a>

`fn` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The function to run; it receives [cancellationToken](index.md#StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken).cancellationToken 'StandSure\.Functional\.Try\.RunAsync\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\>, System\.Threading\.CancellationToken\)\.cancellationToken')\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value returns a `result.null_delegate` failure\.

<a name='StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Passed to [fn](index.md#StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken).fn 'StandSure\.Functional\.Try\.RunAsync\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\>, System\.Threading\.CancellationToken\)\.fn')\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A success holding [Value](../Unit/Value.md 'StandSure\.Functional\.Unit\.Value'), or the captured exception as a failure\.

<a name='StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken)'></a>

## Try\.RunAsync\<T\>\(Func\<CancellationToken,Task\<T\>\>, CancellationToken\) Method

Runs an asynchronous function and captures a thrown exception as a failure\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> RunAsync<T>(System.Func<System.Threading.CancellationToken,System.Threading.Tasks.Task<T>?> fn, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Type parameters

<a name='StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).T'></a>

`T`

The result type\.
#### Parameters

<a name='StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).fn'></a>

`fn` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[T](index.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).T 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The function to run; it receives [cancellationToken](index.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).cancellationToken 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)\.cancellationToken')\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value returns a `result.null_delegate` failure\.

<a name='StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Passed to [fn](index.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).fn 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)\.fn')\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).T 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The task's value as a success, or the captured exception as a failure\. An exception thrown
            synchronously by [fn](index.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken).fn 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)\.fn') is captured the same way as one thrown inside the task\. A
            [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') task returns a `result.null_task` failure\.