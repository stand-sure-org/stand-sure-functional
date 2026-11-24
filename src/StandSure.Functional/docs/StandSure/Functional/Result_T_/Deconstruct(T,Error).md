#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Deconstruct\(T, Error\) Method

Deconstructs Go\-style: `var (value, error) = result;` then check `error` first\.

```csharp
public void Deconstruct(out T? value, out StandSure.Functional.Error? error);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Deconstruct(T,StandSure.Functional.Error).value'></a>

`value` [T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')

The value on success; `default` on failure\.

<a name='StandSure.Functional.Result_T_.Deconstruct(T,StandSure.Functional.Error).error'></a>

`error` [Error](../Error/index.md 'StandSure\.Functional\.Error')

[null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') on success; the error on failure
            \([Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') for `default`\)\.