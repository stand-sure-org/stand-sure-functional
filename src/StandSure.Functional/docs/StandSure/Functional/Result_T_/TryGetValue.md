#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.TryGetValue Method

| Overloads | |
| :--- | :--- |
| [TryGetValue\(Error\)](TryGetValue.md#StandSure.Functional.Result_T_.TryGetValue(StandSure.Functional.Error) 'StandSure\.Functional\.Result\<T\>\.TryGetValue\(StandSure\.Functional\.Error\)') | Gets the error if this result is a failure\. |
| [TryGetValue\(T\)](TryGetValue.md#StandSure.Functional.Result_T_.TryGetValue(T) 'StandSure\.Functional\.Result\<T\>\.TryGetValue\(T\)') | Gets the success value if this result is a success\. |

<a name='StandSure.Functional.Result_T_.TryGetValue(StandSure.Functional.Error)'></a>

## Result\<T\>\.TryGetValue\(Error\) Method

Gets the error if this result is a failure\.

```csharp
public bool TryGetValue(out StandSure.Functional.Error? error);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.TryGetValue(StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

The error \([Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') for `default`\); [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') on success\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') if this result is a failure\.

<a name='StandSure.Functional.Result_T_.TryGetValue(T)'></a>

## Result\<T\>\.TryGetValue\(T\) Method

Gets the success value if this result is a success\.

```csharp
public bool TryGetValue(out T value);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.TryGetValue(T).value'></a>

`value` [T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')

The value; `default` on failure\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') if this result is a success\.