#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## Try Class

Bridges code that throws into [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\. This is the only place in the library that catches exceptions\.

```csharp
public static class Try
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Try

### Remarks

A thrown exception becomes a failure built with [FromException\(Exception\)](../Error/FromException(Exception).md 'StandSure\.Functional\.Error\.FromException\(System\.Exception\)'), with these exceptions:
[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') (including [System\.Threading\.Tasks\.TaskCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcanceledexception 'System\.Threading\.Tasks\.TaskCanceledException')) and
[System\.OutOfMemoryException](https://learn.microsoft.com/en-us/dotnet/api/system.outofmemoryexception 'System\.OutOfMemoryException') always propagate. An [System\.AggregateException](https://learn.microsoft.com/en-us/dotnet/api/system.aggregateexception 'System\.AggregateException') whose flattened inner
exceptions are all [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') (for example from `Task.Wait()`) propagates as its
first inner cancellation, with the original stack trace. Any other [System\.AggregateException](https://learn.microsoft.com/en-us/dotnet/api/system.aggregateexception 'System\.AggregateException') becomes a failure.

A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') delegate returns a failure with code `result.null_delegate` instead of throwing,
            and a `RunAsync` delegate that returns a [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') task returns a `result.null_task` failure.

| Methods | |
| :--- | :--- |
| [Run\(Action\)](Run.md#StandSure.Functional.Try.Run(System.Action) 'StandSure\.Functional\.Try\.Run\(System\.Action\)') | Runs an action and captures a thrown exception as a failure\. |
| [Run\(Func&lt;Task&gt;\)](Run.md#StandSure.Functional.Try.Run(System.Func_System.Threading.Tasks.Task_) 'StandSure\.Functional\.Try\.Run\(System\.Func\<System\.Threading\.Tasks\.Task\>\)') | Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work             ran or failed\. Calling it does not compile; use [RunAsync\(Func&lt;CancellationToken,Task&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\>, System\.Threading\.CancellationToken\)')\. |
| [Run&lt;T&gt;\(Func&lt;Task&lt;T&gt;&gt;\)](Run.md#StandSure.Functional.Try.Run_T_(System.Func_System.Threading.Tasks.Task_T__) 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<System\.Threading\.Tasks\.Task\<T\>\>\)') | Not supported: an asynchronous delegate would succeed as soon as it returned its task, before the work             ran or failed\. Calling it does not compile; use [RunAsync&lt;T&gt;\(Func&lt;CancellationToken,Task&lt;T&gt;&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)')\. |
| [Run&lt;T&gt;\(Func&lt;T&gt;\)](Run.md#StandSure.Functional.Try.Run_T_(System.Func_T_) 'StandSure\.Functional\.Try\.Run\<T\>\(System\.Func\<T\>\)') | Runs a function and captures a thrown exception as a failure\. |
| [RunAsync\(Func&lt;CancellationToken,Task&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\>, System\.Threading\.CancellationToken\)') | Runs an asynchronous function that returns no value and captures a thrown exception as a failure\. |
| [RunAsync&lt;T&gt;\(Func&lt;CancellationToken,Task&lt;T&gt;&gt;, CancellationToken\)](RunAsync.md#StandSure.Functional.Try.RunAsync_T_(System.Func_System.Threading.CancellationToken,System.Threading.Tasks.Task_T__,System.Threading.CancellationToken) 'StandSure\.Functional\.Try\.RunAsync\<T\>\(System\.Func\<System\.Threading\.CancellationToken,System\.Threading\.Tasks\.Task\<T\>\>, System\.Threading\.CancellationToken\)') | Runs an asynchronous function and captures a thrown exception as a failure\. |
