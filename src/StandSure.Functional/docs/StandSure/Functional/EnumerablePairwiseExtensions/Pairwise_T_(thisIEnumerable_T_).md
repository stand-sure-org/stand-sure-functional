#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[EnumerablePairwiseExtensions](index.md 'StandSure\.Functional\.EnumerablePairwiseExtensions')

## EnumerablePairwiseExtensions\.Pairwise\<T\>\(this IEnumerable\<T\>\) Method

Yields each element together with the one before it\.

```csharp
public static System.Collections.Generic.IEnumerable<(T Previous,T Current)> Pairwise<T>(this System.Collections.Generic.IEnumerable<T>? source);
```
#### Type parameters

<a name='StandSure.Functional.EnumerablePairwiseExtensions.Pairwise_T_(thisSystem.Collections.Generic.IEnumerable_T_).T'></a>

`T`

The element type\.
#### Parameters

<a name='StandSure.Functional.EnumerablePairwiseExtensions.Pairwise_T_(thisSystem.Collections.Generic.IEnumerable_T_).source'></a>

`source` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[T](Pairwise_T_(thisIEnumerable_T_).md#StandSure.Functional.EnumerablePairwiseExtensions.Pairwise_T_(thisSystem.Collections.Generic.IEnumerable_T_).T 'StandSure\.Functional\.EnumerablePairwiseExtensions\.Pairwise\<T\>\(this System\.Collections\.Generic\.IEnumerable\<T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The elements\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives an empty sequence and records a
            `result.null_source` error in telemetry; it never throws\.

#### Returns
[System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[T](Pairwise_T_(thisIEnumerable_T_).md#StandSure.Functional.EnumerablePairwiseExtensions.Pairwise_T_(thisSystem.Collections.Generic.IEnumerable_T_).T 'StandSure\.Functional\.EnumerablePairwiseExtensions\.Pairwise\<T\>\(this System\.Collections\.Generic\.IEnumerable\<T\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[T](Pairwise_T_(thisIEnumerable_T_).md#StandSure.Functional.EnumerablePairwiseExtensions.Pairwise_T_(thisSystem.Collections.Generic.IEnumerable_T_).T 'StandSure\.Functional\.EnumerablePairwiseExtensions\.Pairwise\<T\>\(this System\.Collections\.Generic\.IEnumerable\<T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')  
For `[1, 2, 3]`: `(1, 2), (2, 3)`\. An empty or single\-element source gives an empty sequence\.
            Lazy: nothing is enumerated until the result is iterated, and the source is enumerated once\.