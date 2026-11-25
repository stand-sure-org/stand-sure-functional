#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## Result\<T\> Struct

The outcome of an operation that can fail: either a success holding a [T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')
or a failure holding an [Error](../Error/index.md 'StandSure\.Functional\.Error')\.

```csharp
public readonly struct Result<T> : System.Runtime.CompilerServices.IUnion, System.IEquatable<StandSure.Functional.Result<T>>
```
#### Type parameters

<a name='StandSure.Functional.Result_T_.T'></a>

`T`

The success value type\.

Implements [System\.Runtime\.CompilerServices\.IUnion](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.iunion 'System\.Runtime\.CompilerServices\.IUnion'), [System\.IEquatable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1')[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1')

### Remarks

Every combinator treats `default(Result<T>)` as a failure whose error is
[Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized'). A combinator on a failure never calls its delegate.

A delegate that throws is not caught here; only [Try](../Try/index.md 'StandSure\.Functional\.Try') catches. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') delegate
never throws either. `Map`, `Bind`, `Ensure` and `Tap` return a failure with
code `result.null_delegate` when they would have had to call it. `MapError` and `TapError` keep the
original failure and record a `result.null_delegate` failure in telemetry. `Match` returns
`default(TOut)` when the delegate for the active branch is [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null'), and records a
`result.null_delegate` failure in telemetry. A delegate the active
branch does not use may be [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') without effect.

| Constructors | |
| :--- | :--- |
| [Result\(Error\)](Result_T_.md#StandSure.Functional.Result_T_.Result(StandSure.Functional.Error) 'StandSure\.Functional\.Result\<T\>\.Result\(StandSure\.Functional\.Error\)') | Initializes a new instance of the [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>') struct as a failure\. |
| [Result\(T\)](Result_T_.md#StandSure.Functional.Result_T_.Result(T) 'StandSure\.Functional\.Result\<T\>\.Result\(T\)') | Initializes a new instance of the [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>') struct as a success\. |

| Properties | |
| :--- | :--- |
| [HasValue](HasValue.md 'StandSure\.Functional\.Result\<T\>\.HasValue') | Gets a value indicating whether the union holds a case\. Always [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool'),             because the `default` value is the [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') failure\. |
| [IsFailure](IsFailure.md 'StandSure\.Functional\.Result\<T\>\.IsFailure') | Gets a value indicating whether this result is a failure, including the `default` value\. |
| [IsSuccess](IsSuccess.md 'StandSure\.Functional\.Result\<T\>\.IsSuccess') | Gets a value indicating whether this result is a success\. |
| [Value](Value.md 'StandSure\.Functional\.Result\<T\>\.Value') | Gets the current case as an object: the success value, or the [Error](../Error/index.md 'StandSure\.Functional\.Error')\.             Never throws\. Boxes a value\-type [T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T'), so prefer `switch` or `TryGetValue`\. |

| Methods | |
| :--- | :--- |
| [Bind&lt;TOut&gt;\(Func&lt;T,Result&lt;TOut&gt;&gt;\)](Bind_TOut_(Func_T,Result_TOut__).md 'StandSure\.Functional\.Result\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)') | Chains an operation that itself returns a [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>'); a failure short\-circuits\. |
| [Deconstruct\(T, Error\)](Deconstruct(T,Error).md 'StandSure\.Functional\.Result\<T\>\.Deconstruct\(T, StandSure\.Functional\.Error\)') | Deconstructs Go\-style: `var (value, error) = result;` then check `error` first\. |
| [Ensure\(Func&lt;T,bool&gt;, Error\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,StandSure.Functional.Error) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, StandSure\.Functional\.Error\)') | Turns a success into the given failure when the predicate is false\. |
| [Ensure\(Func&lt;T,bool&gt;, string\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,string) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, string\)') | Turns a success into a validation failure when the predicate is false, describing it with the             source text of the predicate\. |
| [Ensure\(Func&lt;T,bool&gt;, Func&lt;T,Error&gt;\)](Ensure.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_) 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)') | Turns a success into a failure built by [error](index.md#StandSure.Functional.Result_T_.Ensure(System.Func_T,bool_,System.Func_T,StandSure.Functional.Error_).error 'StandSure\.Functional\.Result\<T\>\.Ensure\(System\.Func\<T,bool\>, System\.Func\<T,StandSure\.Functional\.Error\>\)\.error') when the predicate is false\. |
| [Equals\(object\)](Equals.md#StandSure.Functional.Result_T_.Equals(object) 'StandSure\.Functional\.Result\<T\>\.Equals\(object\)') | Compares this result with an object\. |
| [Equals\(Result&lt;T&gt;\)](Equals.md#StandSure.Functional.Result_T_.Equals(StandSure.Functional.Result_T_) 'StandSure\.Functional\.Result\<T\>\.Equals\(StandSure\.Functional\.Result\<T\>\)') | Compares this result with another by their contents\. |
| [GetHashCode\(\)](GetHashCode().md 'StandSure\.Functional\.Result\<T\>\.GetHashCode\(\)') | Gets a hash code that agrees with [Equals\(Result&lt;T&gt;\)](Equals.md#StandSure.Functional.Result_T_.Equals(StandSure.Functional.Result_T_) 'StandSure\.Functional\.Result\<T\>\.Equals\(StandSure\.Functional\.Result\<T\>\)')\. |
| [GetValueOrDefault\(T\)](GetValueOrDefault(T).md 'StandSure\.Functional\.Result\<T\>\.GetValueOrDefault\(T\)') | Gets the success value, or a fallback for a failure \(including `default`\)\. |
| [Map&lt;TOut&gt;\(Func&lt;T,TOut&gt;\)](Map_TOut_(Func_T,TOut_).md 'StandSure\.Functional\.Result\<T\>\.Map\<TOut\>\(System\.Func\<T,TOut\>\)') | Transforms the success value; a failure passes through unchanged\. |
| [MapError\(Func&lt;Error,Error&gt;\)](MapError(Func_Error,Error_).md 'StandSure\.Functional\.Result\<T\>\.MapError\(System\.Func\<StandSure\.Functional\.Error,StandSure\.Functional\.Error\>\)') | Transforms the error of a failure; a success passes through unchanged\. |
| [Match&lt;TOut&gt;\(Func&lt;Error,TOut&gt;, Func&lt;T,TOut&gt;\)](Match_TOut_(Func_Error,TOut_,Func_T,TOut_).md 'StandSure\.Functional\.Result\<T\>\.Match\<TOut\>\(System\.Func\<StandSure\.Functional\.Error,TOut\>, System\.Func\<T,TOut\>\)') | Runs one of two functions depending on the case\. The failure function comes first\. |
| [Tap\(Action&lt;T&gt;\)](Tap(Action_T_).md 'StandSure\.Functional\.Result\<T\>\.Tap\(System\.Action\<T\>\)') | Runs a side effect on the success value and returns this result unchanged\. |
| [TapError\(Action&lt;Error&gt;\)](TapError(Action_Error_).md 'StandSure\.Functional\.Result\<T\>\.TapError\(System\.Action\<StandSure\.Functional\.Error\>\)') | Runs a side effect on the error of a failure and returns this result unchanged\. |
| [TryGetValue\(Error\)](TryGetValue.md#StandSure.Functional.Result_T_.TryGetValue(StandSure.Functional.Error) 'StandSure\.Functional\.Result\<T\>\.TryGetValue\(StandSure\.Functional\.Error\)') | Gets the error if this result is a failure\. |
| [TryGetValue\(T\)](TryGetValue.md#StandSure.Functional.Result_T_.TryGetValue(T) 'StandSure\.Functional\.Result\<T\>\.TryGetValue\(T\)') | Gets the success value if this result is a success\. |

| Operators | |
| :--- | :--- |
| [operator ==\(Result&lt;T&gt;, Result&lt;T&gt;\)](operator(Result_T_,Result_T_).md 'StandSure\.Functional\.Result\<T\>\.op\_Equality\(StandSure\.Functional\.Result\<T\>, StandSure\.Functional\.Result\<T\>\)') | Compares two results by their contents\. |
| [operator \!=\(Result&lt;T&gt;, Result&lt;T&gt;\)](operator!(Result_T_,Result_T_).md 'StandSure\.Functional\.Result\<T\>\.op\_Inequality\(StandSure\.Functional\.Result\<T\>, StandSure\.Functional\.Result\<T\>\)') | Compares two results by their contents\. |
