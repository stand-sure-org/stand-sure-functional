#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result](index.md 'StandSure\.Functional\.Result')

## Result\.Fail\<T\>\(Error\) Method

Alias for [Failure&lt;T&gt;\(Error\)](Failure_T_(Error).md 'StandSure\.Functional\.Result\.Failure\<T\>\(StandSure\.Functional\.Error\)'), kept for compatibility with common model output\.
            [Failure&lt;T&gt;\(Error\)](Failure_T_(Error).md 'StandSure\.Functional\.Result\.Failure\<T\>\(StandSure\.Functional\.Error\)') is preferred\.

```csharp
public static StandSure.Functional.Result<T> Fail<T>(StandSure.Functional.Error error);
```
#### Type parameters

<a name='StandSure.Functional.Result.Fail_T_(StandSure.Functional.Error).T'></a>

`T`

The value type the result would have held\.
#### Parameters

<a name='StandSure.Functional.Result.Fail_T_(StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](Fail_T_(Error).md#StandSure.Functional.Result.Fail_T_(StandSure.Functional.Error).T 'StandSure\.Functional\.Result\.Fail\<T\>\(StandSure\.Functional\.Error\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A failed result\.