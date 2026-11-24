#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Ensure Method

| Overloads | |
| :--- | :--- |
| [Ensure\(Func&lt;T,bool&gt;, Error\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,StandSure.Functional.Error) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, StandSure\.Functional\.Error\)') | Turns a success into the given failure when the predicate is false\. |
| [Ensure\(Func&lt;T,bool&gt;, string\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,string) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, string\)') | Turns a success into a validation failure when the predicate is false, describing it with the             source text of the predicate\. |
| [Ensure\(Func&lt;T,bool&gt;, Func&lt;T,Error&gt;\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)') | Turns a success into a failure built by [error](index.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).error 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.error') when the predicate is false\. |

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,StandSure.Functional.Error)'></a>

## Result\<T\>\.Ensure\(Func\<T,bool\>, Error\) Method

Turns a success into the given failure when the predicate is false\.

```csharp
public StandSure.Functional.Result<T> Ensure(System.Func<T,bool> predicate, StandSure.Functional.Error error);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,StandSure.Functional.Error).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The check on the value\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error returned when the predicate is false\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value becomes
            [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\.

#### Returns
[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')  
This result if it is a failure or the predicate holds; otherwise a failure with [error](index.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,StandSure.Functional.Error).error 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, StandSure\.Functional\.Error\)\.error')\.

### Remarks
The [Error](../Error/index.md 'StandSure\.Functional\.Error') is built before the call and is recorded in telemetry when built, even if the
            predicate holds; prefer the factory overload [Ensure\(Func&lt;T,bool&gt;, Func&lt;T,Error&gt;\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)')\.

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,string)'></a>

## Result\<T\>\.Ensure\(Func\<T,bool\>, string\) Method

Turns a success into a validation failure when the predicate is false, describing it with the
            source text of the predicate\.

```csharp
public StandSure.Functional.Result<T> Ensure(System.Func<T,bool> predicate, string? expression=null);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,string).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The check on the value\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,string).expression'></a>

`expression` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Filled in by the compiler with the source text of [predicate](index.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,string).predicate 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, string\)\.predicate');
            do not pass it\. When [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null'), the message is generic\.

#### Returns
[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')  
This result if it is a failure or the predicate holds; otherwise a [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation')
            failure with code `result.ensure_failed` and [expression](index.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,string).expression 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, string\)\.expression') as its message\.

### Remarks
Hidden from IntelliSense: the message is source code \(`"u => u.Age >= 13"`\), and an HTTP mapping can
show it to users\. Prefer [Ensure\(Func&lt;T,bool&gt;, Func&lt;T,Error&gt;\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)') with an error that has a stable
code and a human message\.

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_)'></a>

## Result\<T\>\.Ensure\(Func\<T,bool\>, Func\<T,Error\>\) Method

Turns a success into a failure built by [error](index.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).error 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.error') when the predicate is false\.

```csharp
public StandSure.Functional.Result<T> Ensure(System.Func<T,bool> predicate, System.Func<T,StandSure.Functional.Error> error);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).predicate'></a>

`predicate` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The check on the value\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

<a name='StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).error'></a>

`error` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Builds the error from the value; called only when the predicate is false, so nothing is
            built or recorded on the success path\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives a `result.null_delegate`
            failure when it would be called; a [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') return becomes [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\.

#### Returns
[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')  
This result if it is a failure or the predicate holds; otherwise a failure with the built error\.