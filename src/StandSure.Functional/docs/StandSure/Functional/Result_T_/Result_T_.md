#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\> Constructors

| Overloads | |
| :--- | :--- |
| [Result\(Error\)](Result_T_.md#StandSure.Functional.Result_T_.Result(StandSure.Functional.Error) 'StandSure\.Functional\.Result\<T\>\.Result\(StandSure\.Functional\.Error\)') | Initializes a new instance of the [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>') struct as a failure\. |
| [Result\(T\)](Result_T_.md#StandSure.Functional.Result_T_.Result(T) 'StandSure\.Functional\.Result\<T\>\.Result\(T\)') | Initializes a new instance of the [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>') struct as a success\. |

<a name='StandSure.Functional.Result_T_.Result(StandSure.Functional.Error)'></a>

## Result\(Error\) Constructor

Initializes a new instance of the [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>') struct as a failure\.

```csharp
public Result(StandSure.Functional.Error? error);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Result(StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') error is replaced with [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\.

<a name='StandSure.Functional.Result_T_.Result(T)'></a>

## Result\(T\) Constructor

Initializes a new instance of the [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>') struct as a success\.

```csharp
public Result(T value);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Result(T).value'></a>

`value` [T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')

The success value; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') is allowed and is still a success\.