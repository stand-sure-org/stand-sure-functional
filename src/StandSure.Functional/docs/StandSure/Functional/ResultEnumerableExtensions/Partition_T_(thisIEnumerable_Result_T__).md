#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultEnumerableExtensions](index.md 'StandSure\.Functional\.ResultEnumerableExtensions')

## ResultEnumerableExtensions\.Partition\<T\>\(this IEnumerable\<Result\<T\>\>\) Method

Separates the successes from the failures, keeping the order of each\.

```csharp
public static (System.Collections.Generic.IReadOnlyList<T> Successes,System.Collections.Generic.IReadOnlyList<StandSure.Functional.Error> Failures) Partition<T>(this System.Collections.Generic.IEnumerable<StandSure.Functional.Result<T>>? source);
```
#### Type parameters

<a name='StandSure.Functional.ResultEnumerableExtensions.Partition_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultEnumerableExtensions.Partition_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).source'></a>

`source` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](Partition_T_(thisIEnumerable_Result_T__).md#StandSure.Functional.ResultEnumerableExtensions.Partition_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).T 'StandSure\.Functional\.ResultEnumerableExtensions\.Partition\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The results\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives two empty lists\.

#### Returns
[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[T](Partition_T_(thisIEnumerable_Result_T__).md#StandSure.Functional.ResultEnumerableExtensions.Partition_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).T 'StandSure\.Functional\.ResultEnumerableExtensions\.Partition\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')  
The success values, then the failure errors\.