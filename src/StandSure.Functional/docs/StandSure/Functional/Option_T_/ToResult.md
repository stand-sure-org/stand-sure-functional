#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.ToResult Method

| Overloads | |
| :--- | :--- |
| [ToResult\(Error\)](ToResult.md#StandSure.Functional.Option_T_.ToResult(StandSure.Functional.Error) 'StandSure\.Functional\.Option\<T\>\.ToResult\(StandSure\.Functional\.Error\)') | Converts to a [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\. |
| [ToResult\(Func&lt;Error&gt;\)](ToResult.md#StandSure.Functional.Option_T_.ToResult(System.Func_StandSure.Functional.Error_) 'StandSure\.Functional\.Option\<T\>\.ToResult\(System\.Func\<StandSure\.Functional\.Error\>\)') | Converts to a [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>'), building the error only for none\. |

<a name='StandSure.Functional.Option_T_.ToResult(StandSure.Functional.Error)'></a>

## Option\<T\>\.ToResult\(Error\) Method

Converts to a [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\.

```csharp
public StandSure.Functional.Result<T> ToResult(StandSure.Functional.Error ifNone);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.ToResult(StandSure.Functional.Error).ifNone'></a>

`ifNone` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error for none\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value becomes
            [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A success holding the value, or a failure with [ifNone](index.md#StandSure.Functional.Option_T_.ToResult(StandSure.Functional.Error).ifNone 'StandSure\.Functional\.Option\<T\>\.ToResult\(StandSure\.Functional\.Error\)\.ifNone')\.

### Remarks
The [Error](../Error/index.md 'StandSure\.Functional\.Error') is built before the call and is recorded in telemetry when built, even if this
            option has a value; prefer the factory overload [ToResult\(Func&lt;Error&gt;\)](ToResult.md#StandSure.Functional.Option_T_.ToResult(System.Func_StandSure.Functional.Error_) 'StandSure\.Functional\.Option\<T\>\.ToResult\(System\.Func\<StandSure\.Functional\.Error\>\)')\.

<a name='StandSure.Functional.Option_T_.ToResult(System.Func_StandSure.Functional.Error_)'></a>

## Option\<T\>\.ToResult\(Func\<Error\>\) Method

Converts to a [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>'), building the error only for none\.

```csharp
public StandSure.Functional.Result<T> ToResult(System.Func<StandSure.Functional.Error> ifNone);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.ToResult(System.Func_StandSure.Functional.Error_).ifNone'></a>

`ifNone` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

Builds the error for none; not called when there is a value, so nothing is built or
            recorded then\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives a `result.null_delegate` failure for none; a
            [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') return becomes [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A success holding the value, or a failure with the built error\.