#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result](index.md 'StandSure\.Functional\.Result')

## Result\.Success Method

| Overloads | |
| :--- | :--- |
| [Success\(\)](Success.md#StandSure.Functional.Result.Success() 'StandSure\.Functional\.Result\.Success\(\)') | Creates a success that carries no value\. |
| [Success&lt;T&gt;\(T\)](Success.md#StandSure.Functional.Result.Success_T_(T) 'StandSure\.Functional\.Result\.Success\<T\>\(T\)') | Creates a success\. |

<a name='StandSure.Functional.Result.Success()'></a>

## Result\.Success\(\) Method

Creates a success that carries no value\.

```csharp
public static StandSure.Functional.Result<StandSure.Functional.Unit> Success();
```

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A successful [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>') of [Unit](../Unit/index.md 'StandSure\.Functional\.Unit')\.

<a name='StandSure.Functional.Result.Success_T_(T)'></a>

## Result\.Success\<T\>\(T\) Method

Creates a success\.

```csharp
public static StandSure.Functional.Result<T> Success<T>(T value);
```
#### Type parameters

<a name='StandSure.Functional.Result.Success_T_(T).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.Result.Success_T_(T).value'></a>

`value` [T](index.md#StandSure.Functional.Result.Success_T_(T).T 'StandSure\.Functional\.Result\.Success\<T\>\(T\)\.T')

The value\. When [T](index.md#StandSure.Functional.Result.Success_T_(T).T 'StandSure\.Functional\.Result\.Success\<T\>\(T\)\.T') is [Error](../Error/index.md 'StandSure\.Functional\.Error') or [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object'),
            the result is still a success, whatever the value's runtime type\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result.Success_T_(T).T 'StandSure\.Functional\.Result\.Success\<T\>\(T\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A successful result\.