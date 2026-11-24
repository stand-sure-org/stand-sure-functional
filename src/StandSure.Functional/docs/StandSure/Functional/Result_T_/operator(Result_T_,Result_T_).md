#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.operator ==\(Result\<T\>, Result\<T\>\) Operator

Compares two results by their contents\.

```csharp
public static bool operator ==(StandSure.Functional.Result<T> left, StandSure.Functional.Result<T> right);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.op_Equality(StandSure.Functional.Result_T_,StandSure.Functional.Result_T_).left'></a>

`left` [StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

The first result\.

<a name='StandSure.Functional.Result_T_.op_Equality(StandSure.Functional.Result_T_,StandSure.Functional.Result_T_).right'></a>

`right` [StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

The second result\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') when both hold the same outcome\.

### Remarks
Two successes are equal when their values are \([System\.Collections\.Generic\.EqualityComparer&lt;&gt;\.Default](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.equalitycomparer-1.default 'System\.Collections\.Generic\.EqualityComparer\`1\.Default')\); two failures are equal
when their errors are\. `default(Result<T>)` equals a failure holding [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\.