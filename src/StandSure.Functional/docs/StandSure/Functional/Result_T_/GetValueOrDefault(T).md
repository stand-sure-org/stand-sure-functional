#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.GetValueOrDefault\(T\) Method

Gets the success value, or a fallback for a failure \(including `default`\)\.

```csharp
public T GetValueOrDefault(T fallback);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.GetValueOrDefault(T).fallback'></a>

`fallback` [T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')

The value returned when this result is not a success\.

#### Returns
[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')  
The value or [fallback](GetValueOrDefault(T).md#StandSure.Functional.Result_T_.GetValueOrDefault(T).fallback 'StandSure\.Functional\.Result\<T\>\.GetValueOrDefault\(T\)\.fallback')\.