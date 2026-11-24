#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## Option\<T\> Struct

An optional value: either some [T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T') or [None](../None/index.md 'StandSure\.Functional\.None')\.

```csharp
public readonly struct Option<T> : System.Runtime.CompilerServices.IUnion, System.IEquatable<StandSure.Functional.Option<T>>
```
#### Type parameters

<a name='StandSure.Functional.Option_T_.T'></a>

`T`

The value type\.

Implements [System\.Runtime\.CompilerServices\.IUnion](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.iunion 'System\.Runtime\.CompilerServices\.IUnion'), [System\.IEquatable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1')[StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.iequatable-1 'System\.IEquatable\`1')

### Remarks

Every combinator treats `default(Option<T>)` as none. A combinator on none never calls its delegate.

A delegate that throws is not caught here; only [Try](../Try/index.md 'StandSure\.Functional\.Try') catches. An option has no error channel, so a
[null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') delegate that would have been called makes `Map`, `Bind` and `Filter` return
none and records a `result.null_delegate` failure in telemetry. `Match` returns `default(TOut)`
when the delegate for the active branch is [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null'), and records the same failure. A delegate the
active branch does not use may be [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') without effect.

| Constructors | |
| :--- | :--- |
| [Option\(None\)](Option_T_.md#StandSure.Functional.Option_T_.Option(StandSure.Functional.None) 'StandSure\.Functional\.Option\<T\>\.Option\(StandSure\.Functional\.None\)') | Initializes a new instance of the [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>') struct as none\. |
| [Option\(T\)](Option_T_.md#StandSure.Functional.Option_T_.Option(T) 'StandSure\.Functional\.Option\<T\>\.Option\(T\)') | Initializes a new instance of the [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>') struct holding a value\. |

| Properties | |
| :--- | :--- |
| [HasValue](HasValue.md 'StandSure\.Functional\.Option\<T\>\.HasValue') | Gets a value indicating whether the union holds a case\. Always [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool'),             because the `default` value is none\. |
| [IsNone](IsNone.md 'StandSure\.Functional\.Option\<T\>\.IsNone') | Gets a value indicating whether this option is none, including the `default` value\. |
| [IsSome](IsSome.md 'StandSure\.Functional\.Option\<T\>\.IsSome') | Gets a value indicating whether this option holds a value\. |
| [None](None.md 'StandSure\.Functional\.Option\<T\>\.None') | Gets the none option\. |
| [Value](Value.md 'StandSure\.Functional\.Option\<T\>\.Value') | Gets the current case as an object: the value, or [Instance](../None/Instance.md 'StandSure\.Functional\.None\.Instance')             when absent\. Never throws\. Boxes a value\-type [T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T'), so prefer `switch` or             `TryGetValue`\. |

| Methods | |
| :--- | :--- |
| [Bind&lt;TOut&gt;\(Func&lt;T,Option&lt;TOut&gt;&gt;\)](Bind_TOut_(Func_T,Option_TOut__).md 'StandSure\.Functional\.Option\<T\>\.Bind\<TOut\>\(System\.Func\<T,StandSure\.Functional\.Option\<TOut\>\>\)') | Chains an operation that itself returns an [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>'); none short\-circuits\. |
| [Equals\(object\)](Equals.md#StandSure.Functional.Option_T_.Equals(object) 'StandSure\.Functional\.Option\<T\>\.Equals\(object\)') | Compares this option with an object\. |
| [Equals\(Option&lt;T&gt;\)](Equals.md#StandSure.Functional.Option_T_.Equals(StandSure.Functional.Option_T_) 'StandSure\.Functional\.Option\<T\>\.Equals\(StandSure\.Functional\.Option\<T\>\)') | Compares this option with another by their contents\. |
| [Filter\(Func&lt;T,bool&gt;\)](Filter(Func_T,bool_).md 'StandSure\.Functional\.Option\<T\>\.Filter\(System\.Func\<T,bool\>\)') | Keeps some only when the predicate holds\. |
| [GetHashCode\(\)](GetHashCode().md 'StandSure\.Functional\.Option\<T\>\.GetHashCode\(\)') | Gets a hash code that agrees with [Equals\(Option&lt;T&gt;\)](Equals.md#StandSure.Functional.Option_T_.Equals(StandSure.Functional.Option_T_) 'StandSure\.Functional\.Option\<T\>\.Equals\(StandSure\.Functional\.Option\<T\>\)')\. |
| [GetValueOrDefault\(T\)](GetValueOrDefault(T).md 'StandSure\.Functional\.Option\<T\>\.GetValueOrDefault\(T\)') | Gets the value, or a fallback for none \(including `default`\)\. |
| [Map&lt;TOut&gt;\(Func&lt;T,TOut&gt;\)](Map_TOut_(Func_T,TOut_).md 'StandSure\.Functional\.Option\<T\>\.Map\<TOut\>\(System\.Func\<T,TOut\>\)') | Transforms the value; none passes through unchanged\. |
| [Match&lt;TOut&gt;\(Func&lt;TOut&gt;, Func&lt;T,TOut&gt;\)](Match_TOut_(Func_TOut_,Func_T,TOut_).md 'StandSure\.Functional\.Option\<T\>\.Match\<TOut\>\(System\.Func\<TOut\>, System\.Func\<T,TOut\>\)') | Runs one of two functions depending on the case\. The none function comes first\. |
| [ToResult\(Error\)](ToResult.md#StandSure.Functional.Option_T_.ToResult(StandSure.Functional.Error) 'StandSure\.Functional\.Option\<T\>\.ToResult\(StandSure\.Functional\.Error\)') | Converts to a [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\. |
| [ToResult\(Func&lt;Error&gt;\)](ToResult.md#StandSure.Functional.Option_T_.ToResult(System.Func_StandSure.Functional.Error_) 'StandSure\.Functional\.Option\<T\>\.ToResult\(System\.Func\<StandSure\.Functional\.Error\>\)') | Converts to a [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>'), building the error only for none\. |
| [TryGetValue\(None\)](TryGetValue.md#StandSure.Functional.Option_T_.TryGetValue(StandSure.Functional.None) 'StandSure\.Functional\.Option\<T\>\.TryGetValue\(StandSure\.Functional\.None\)') | Gets the none case if this option is none\. |
| [TryGetValue\(T\)](TryGetValue.md#StandSure.Functional.Option_T_.TryGetValue(T) 'StandSure\.Functional\.Option\<T\>\.TryGetValue\(T\)') | Gets the value if this option is some\. |

| Operators | |
| :--- | :--- |
| [operator ==\(Option&lt;T&gt;, Option&lt;T&gt;\)](operator(Option_T_,Option_T_).md 'StandSure\.Functional\.Option\<T\>\.op\_Equality\(StandSure\.Functional\.Option\<T\>, StandSure\.Functional\.Option\<T\>\)') | Compares two options by their contents\. |
| [operator \!=\(Option&lt;T&gt;, Option&lt;T&gt;\)](operator!(Option_T_,Option_T_).md 'StandSure\.Functional\.Option\<T\>\.op\_Inequality\(StandSure\.Functional\.Option\<T\>, StandSure\.Functional\.Option\<T\>\)') | Compares two options by their contents\. |
