#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Equals Method

| Overloads | |
| :--- | :--- |
| [Equals\(object\)](Equals.md#StandSure.Functional.Result_T_.Equals(object) 'StandSure\.Functional\.Result\<T\>\.Equals\(object\)') | Compares this result with an object\. |
| [Equals\(Result&lt;T&gt;\)](Equals.md#StandSure.Functional.Result_T_.Equals(StandSure.Functional.Result_T_) 'StandSure\.Functional\.Result\<T\>\.Equals\(StandSure\.Functional\.Result\<T\>\)') | Compares this result with another by their contents\. |

<a name='StandSure.Functional.Result_T_.Equals(object)'></a>

## Result\<T\>\.Equals\(object\) Method

Compares this result with an object\.

```csharp
public override bool Equals(object? obj);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Equals(object).obj'></a>

`obj` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to compare with\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') when [obj](index.md#StandSure.Functional.Result_T_.Equals(object).obj 'StandSure\.Functional\.Result\<T\>\.Equals\(object\)\.obj') is a [Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>') holding the same outcome\.

<a name='StandSure.Functional.Result_T_.Equals(StandSure.Functional.Result_T_)'></a>

## Result\<T\>\.Equals\(Result\<T\>\) Method

Compares this result with another by their contents\.

```csharp
public bool Equals(StandSure.Functional.Result<T> other);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Equals(StandSure.Functional.Result_T_).other'></a>

`other` [StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

The result to compare with\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') when both are successes with equal values, or both are failures with equal errors\.