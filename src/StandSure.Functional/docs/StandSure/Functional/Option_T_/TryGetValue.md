#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.TryGetValue Method

| Overloads | |
| :--- | :--- |
| [TryGetValue\(None\)](TryGetValue.md#StandSure.Functional.Option_T_.TryGetValue(StandSure.Functional.None) 'StandSure\.Functional\.Option\<T\>\.TryGetValue\(StandSure\.Functional\.None\)') | Gets the none case if this option is none\. |
| [TryGetValue\(T\)](TryGetValue.md#StandSure.Functional.Option_T_.TryGetValue(T) 'StandSure\.Functional\.Option\<T\>\.TryGetValue\(T\)') | Gets the value if this option is some\. |

<a name='StandSure.Functional.Option_T_.TryGetValue(StandSure.Functional.None)'></a>

## Option\<T\>\.TryGetValue\(None\) Method

Gets the none case if this option is none\.

```csharp
public bool TryGetValue(out StandSure.Functional.None? none);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.TryGetValue(StandSure.Functional.None).none'></a>

`none` [None](../None/index.md 'StandSure\.Functional\.None')

[Instance](../None/Instance.md 'StandSure\.Functional\.None\.Instance') when none; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') otherwise\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') if this option is none\.

<a name='StandSure.Functional.Option_T_.TryGetValue(T)'></a>

## Option\<T\>\.TryGetValue\(T\) Method

Gets the value if this option is some\.

```csharp
public bool TryGetValue(out T value);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.TryGetValue(T).value'></a>

`value` [T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')

The value; `default` when none\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') if this option is some\.