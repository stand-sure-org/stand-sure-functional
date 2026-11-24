#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultTaskExtensions](index.md 'StandSure\.Functional\.ResultTaskExtensions')

## ResultTaskExtensions\.EnsureAsync Method

| Overloads | |
| :--- | :--- |
| [EnsureAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,bool&gt;, Error\)](EnsureAsync.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error) 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, StandSure\.Functional\.Error\)') | Turns an awaited success into the given failure when the predicate is false\. |
| [EnsureAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,bool&gt;, Func&lt;T,Error&gt;\)](EnsureAsync.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)') | Turns an awaited success into a failure built by [error](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).error 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.error') when the predicate is false\. |
| [EnsureAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,Task&lt;bool&gt;&gt;, Error\)](EnsureAsync.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error) 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, StandSure\.Functional\.Error\)') | Turns an awaited success into the given failure when the asynchronous predicate is false\. |
| [EnsureAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,Task&lt;bool&gt;&gt;, Func&lt;T,Error&gt;\)](EnsureAsync.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)') | Turns an awaited success into a failure built by [error](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).error 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.error') when the asynchronous predicate is false\. |

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error)'></a>

## ResultTaskExtensions\.EnsureAsync\<T\>\(this Task\<Result\<T\>\>, Func\<T,bool\>, Error\) Method

Turns an awaited success into the given failure when the predicate is false\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> EnsureAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,bool> predicate, StandSure.Functional.Error error);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, StandSure\.Functional\.Error\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, StandSure\.Functional\.Error\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The check on the value\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error returned when the predicate is false\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, StandSure\.Functional\.Error\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The original result when it is a failure or the predicate holds; otherwise a failure with [error](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,StandSure.Functional.Error).error 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, StandSure\.Functional\.Error\)\.error')\.

### Remarks
The [Error](../Error/index.md 'StandSure\.Functional\.Error') is built before the call and is recorded in telemetry when built, even if the
            predicate holds; prefer the factory overload [EnsureAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,bool&gt;, Func&lt;T,Error&gt;\)](EnsureAsync.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)')\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_)'></a>

## ResultTaskExtensions\.EnsureAsync\<T\>\(this Task\<Result\<T\>\>, Func\<T,bool\>, Func\<T,Error\>\) Method

Turns an awaited success into a failure built by [error](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).error 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.error') when the predicate is false\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> EnsureAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,bool> predicate, System.Func<T,StandSure.Functional.Error> error);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The check on the value\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).error'></a>

`error` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Builds the error from the value; called only when the predicate is false, so nothing is
            built or recorded on the success path\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives a `result.null_delegate`
            failure when it would be called\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The original result when it is a failure or the predicate holds; otherwise a failure with the built error\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error)'></a>

## ResultTaskExtensions\.EnsureAsync\<T\>\(this Task\<Result\<T\>\>, Func\<T,Task\<bool\>\>, Error\) Method

Turns an awaited success into the given failure when the asynchronous predicate is false\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> EnsureAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,System.Threading.Tasks.Task<bool>?> predicate, StandSure.Functional.Error error);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, StandSure\.Functional\.Error\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, StandSure\.Functional\.Error\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The asynchronous check on the value\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error returned when the predicate is false\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, StandSure\.Functional\.Error\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The original result when it is a failure or the predicate holds; otherwise a failure with [error](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,StandSure.Functional.Error).error 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, StandSure\.Functional\.Error\)\.error')\.

### Remarks
The [Error](../Error/index.md 'StandSure\.Functional\.Error') is built before the call and is recorded in telemetry when built, even if the
            predicate holds; prefer the factory overload [EnsureAsync&lt;T&gt;\(this Task&lt;Result&lt;T&gt;&gt;, Func&lt;T,Task&lt;bool&gt;&gt;, Func&lt;T,Error&gt;\)](EnsureAsync.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)')\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_)'></a>

## ResultTaskExtensions\.EnsureAsync\<T\>\(this Task\<Result\<T\>\>, Func\<T,Task\<bool\>\>, Func\<T,Error\>\) Method

Turns an awaited success into a failure built by [error](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).error 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.error') when the asynchronous predicate is false\.

```csharp
public static System.Threading.Tasks.Task<StandSure.Functional.Result<T>> EnsureAsync<T>(this System.Threading.Tasks.Task<StandSure.Functional.Result<T>> source, System.Func<T,System.Threading.Tasks.Task<bool>?> predicate, System.Func<T,StandSure.Functional.Error> error);
```
#### Type parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).source'></a>

`source` [System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

The pending result\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The asynchronous check on the value\.

<a name='StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).error'></a>

`error` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Builds the error from the value; called only when the predicate is false, so nothing is
            built or recorded on the success path\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives a `result.null_delegate`
            failure when it would be called\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.ResultTaskExtensions.EnsureAsync_T_(thisSystem.Threading.Tasks.Task_StandSure.Functional.Result_T__,System.Func_T,System.Threading.Tasks.Task_bool__,System.Func_T,StandSure.Functional.Error_).T 'StandSure\.Functional\.ResultTaskExtensions\.EnsureAsync\<T\>\(this System\.Threading\.Tasks\.Task\<StandSure\.Functional\.Result\<T\>\>, System\.Func\<T,System\.Threading\.Tasks\.Task\<bool\>\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The original result when it is a failure or the predicate holds; otherwise a failure with the built error\.