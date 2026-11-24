#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result](index.md 'StandSure\.Functional\.Result')

## Result\.Ok Method

| Overloads | |
| :--- | :--- |
| [Ok\(\)](Ok.md#StandSure.Functional.Result.Ok() 'StandSure\.Functional\.Result\.Ok\(\)') | Alias for [Success\(\)](Success.md#StandSure.Functional.Result.Success() 'StandSure\.Functional\.Result\.Success\(\)'), kept for compatibility with common model output\.             [Success\(\)](Success.md#StandSure.Functional.Result.Success() 'StandSure\.Functional\.Result\.Success\(\)') is preferred\. |
| [Ok&lt;T&gt;\(T\)](Ok.md#StandSure.Functional.Result.Ok_T_(T) 'StandSure\.Functional\.Result\.Ok\<T\>\(T\)') | Alias for [Success&lt;T&gt;\(T\)](Success.md#StandSure.Functional.Result.Success_T_(T) 'StandSure\.Functional\.Result\.Success\<T\>\(T\)'), kept for compatibility with common model output\.             [Success&lt;T&gt;\(T\)](Success.md#StandSure.Functional.Result.Success_T_(T) 'StandSure\.Functional\.Result\.Success\<T\>\(T\)') is preferred\. |

<a name='StandSure.Functional.Result.Ok()'></a>

## Result\.Ok\(\) Method

Alias for [Success\(\)](Success.md#StandSure.Functional.Result.Success() 'StandSure\.Functional\.Result\.Success\(\)'), kept for compatibility with common model output\.
            [Success\(\)](Success.md#StandSure.Functional.Result.Success() 'StandSure\.Functional\.Result\.Success\(\)') is preferred\.

```csharp
public static StandSure.Functional.Result<StandSure.Functional.Unit> Ok();
```

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A successful [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>') of [Unit](../Unit/index.md 'StandSure\.Functional\.Unit')\.

<a name='StandSure.Functional.Result.Ok_T_(T)'></a>

## Result\.Ok\<T\>\(T\) Method

Alias for [Success&lt;T&gt;\(T\)](Success.md#StandSure.Functional.Result.Success_T_(T) 'StandSure\.Functional\.Result\.Success\<T\>\(T\)'), kept for compatibility with common model output\.
            [Success&lt;T&gt;\(T\)](Success.md#StandSure.Functional.Result.Success_T_(T) 'StandSure\.Functional\.Result\.Success\<T\>\(T\)') is preferred\.

```csharp
public static StandSure.Functional.Result<T> Ok<T>(T value);
```
#### Type parameters

<a name='StandSure.Functional.Result.Ok_T_(T).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.Result.Ok_T_(T).value'></a>

`value` [T](index.md#StandSure.Functional.Result.Ok_T_(T).T 'StandSure\.Functional\.Result\.Ok\<T\>\(T\)\.T')

The value\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result.Ok_T_(T).T 'StandSure\.Functional\.Result\.Ok\<T\>\(T\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A successful result\.