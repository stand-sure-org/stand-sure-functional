#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[OptionEnumerableExtensions](index.md 'StandSure\.Functional\.OptionEnumerableExtensions')

## OptionEnumerableExtensions\.Choose\<T\>\(this IEnumerable\<Option\<T\>\>\) Method

Yields the value of every some, skipping every none \(including `default`\)\.

```csharp
public static System.Collections.Generic.IEnumerable<T> Choose<T>(this System.Collections.Generic.IEnumerable<StandSure.Functional.Option<T>>? source);
```
#### Type parameters

<a name='StandSure.Functional.OptionEnumerableExtensions.Choose_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Option_T__).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.OptionEnumerableExtensions.Choose_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Option_T__).source'></a>

`source` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[StandSure\.Functional\.Option&lt;](../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')[T](Choose_T_(thisIEnumerable_Option_T__).md#StandSure.Functional.OptionEnumerableExtensions.Choose_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Option_T__).T 'StandSure\.Functional\.OptionEnumerableExtensions\.Choose\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Option\<T\>\>\)\.T')[&gt;](../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The options\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives an empty sequence and records a
            `result.null_source` error in telemetry; it never throws\.

#### Returns
[System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[T](Choose_T_(thisIEnumerable_Option_T__).md#StandSure.Functional.OptionEnumerableExtensions.Choose_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Option_T__).T 'StandSure\.Functional\.OptionEnumerableExtensions\.Choose\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Option\<T\>\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')  
A lazy sequence: nothing is enumerated until the result is iterated, and the source is enumerated once\.