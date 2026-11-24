#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.Equals Method

| Overloads | |
| :--- | :--- |
| [Equals\(object\)](Equals.md#StandSure.Functional.Option_T_.Equals(object) 'StandSure\.Functional\.Option\<T\>\.Equals\(object\)') | Compares this option with an object\. |
| [Equals\(Option&lt;T&gt;\)](Equals.md#StandSure.Functional.Option_T_.Equals(StandSure.Functional.Option_T_) 'StandSure\.Functional\.Option\<T\>\.Equals\(StandSure\.Functional\.Option\<T\>\)') | Compares this option with another by their contents\. |

<a name='StandSure.Functional.Option_T_.Equals(object)'></a>

## Option\<T\>\.Equals\(object\) Method

Compares this option with an object\.

```csharp
public override bool Equals(object? obj);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.Equals(object).obj'></a>

`obj` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to compare with\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') when [obj](index.md#StandSure.Functional.Option_T_.Equals(object).obj 'StandSure\.Functional\.Option\<T\>\.Equals\(object\)\.obj') is an [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>') with the same contents\.

<a name='StandSure.Functional.Option_T_.Equals(StandSure.Functional.Option_T_)'></a>

## Option\<T\>\.Equals\(Option\<T\>\) Method

Compares this option with another by their contents\.

```csharp
public bool Equals(StandSure.Functional.Option<T> other);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.Equals(StandSure.Functional.Option_T_).other'></a>

`other` [StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

The option to compare with\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') when both are none, or both hold equal values\.