#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result](index.md 'StandSure\.Functional\.Result')

## Result\.Failure\<T\>\(Error\) Method

Creates a failure\.

```csharp
public static StandSure.Functional.Result<T> Failure<T>(StandSure.Functional.Error error);
```
#### Type parameters

<a name='StandSure.Functional.Result.Failure_T_(StandSure.Functional.Error).T'></a>

`T`

The value type the result would have held\.
#### Parameters

<a name='StandSure.Functional.Result.Failure_T_(StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') error becomes [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](Failure_T_(Error).md#StandSure.Functional.Result.Failure_T_(StandSure.Functional.Error).T 'StandSure\.Functional\.Result\.Failure\<T\>\(StandSure\.Functional\.Error\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A failed result\.