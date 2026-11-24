#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\>\.operator \!=\(Option\<T\>, Option\<T\>\) Operator

Compares two options by their contents\.

```csharp
public static bool operator !=(StandSure.Functional.Option<T> left, StandSure.Functional.Option<T> right);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.op_Inequality(StandSure.Functional.Option_T_,StandSure.Functional.Option_T_).left'></a>

`left` [StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

The first option\.

<a name='StandSure.Functional.Option_T_.op_Inequality(StandSure.Functional.Option_T_,StandSure.Functional.Option_T_).right'></a>

`right` [StandSure\.Functional\.Option&lt;](index.md 'StandSure\.Functional\.Option\<T\>')[T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

The second option\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') when one is none and the other is not, or both hold different values\.