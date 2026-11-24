#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[EnumerableScanExtensions](index.md 'StandSure\.Functional\.EnumerableScanExtensions')

## EnumerableScanExtensions\.Scan\<T,TAcc\>\(this IEnumerable\<T\>, TAcc, Func\<TAcc,T,TAcc\>\) Method

Like `Aggregate`, but yields every running value\.

```csharp
public static System.Collections.Generic.IEnumerable<TAcc> Scan<T,TAcc>(this System.Collections.Generic.IEnumerable<T>? source, TAcc seed, System.Func<TAcc,T,TAcc> accumulator);
```
#### Type parameters

<a name='StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).T'></a>

`T`

The element type\.

<a name='StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).TAcc'></a>

`TAcc`

The accumulator type\.
#### Parameters

<a name='StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).source'></a>

`source` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[T](Scan_T,TAcc_(thisIEnumerable_T_,TAcc,Func_TAcc,T,TAcc_).md#StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).T 'StandSure\.Functional\.EnumerableScanExtensions\.Scan\<T,TAcc\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, TAcc, System\.Func\<TAcc,T,TAcc\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The elements\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives an empty sequence\.

<a name='StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).seed'></a>

`seed` [TAcc](Scan_T,TAcc_(thisIEnumerable_T_,TAcc,Func_TAcc,T,TAcc_).md#StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).TAcc 'StandSure\.Functional\.EnumerableScanExtensions\.Scan\<T,TAcc\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, TAcc, System\.Func\<TAcc,T,TAcc\>\)\.TAcc')

The starting accumulator\. It is \<b\>not\</b\> yielded\.

<a name='StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).accumulator'></a>

`accumulator` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[TAcc](Scan_T,TAcc_(thisIEnumerable_T_,TAcc,Func_TAcc,T,TAcc_).md#StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).TAcc 'StandSure\.Functional\.EnumerableScanExtensions\.Scan\<T,TAcc\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, TAcc, System\.Func\<TAcc,T,TAcc\>\)\.TAcc')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[T](Scan_T,TAcc_(thisIEnumerable_T_,TAcc,Func_TAcc,T,TAcc_).md#StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).T 'StandSure\.Functional\.EnumerableScanExtensions\.Scan\<T,TAcc\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, TAcc, System\.Func\<TAcc,T,TAcc\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[TAcc](Scan_T,TAcc_(thisIEnumerable_T_,TAcc,Func_TAcc,T,TAcc_).md#StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).TAcc 'StandSure\.Functional\.EnumerableScanExtensions\.Scan\<T,TAcc\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, TAcc, System\.Func\<TAcc,T,TAcc\>\)\.TAcc')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')

Combines the running value with the next element\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value
            gives an empty sequence\. A throwing accumulator propagates when the result is iterated\.

#### Returns
[System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[TAcc](Scan_T,TAcc_(thisIEnumerable_T_,TAcc,Func_TAcc,T,TAcc_).md#StandSure.Functional.EnumerableScanExtensions.Scan_T,TAcc_(thisSystem.Collections.Generic.IEnumerable_T_,TAcc,System.Func_TAcc,T,TAcc_).TAcc 'StandSure\.Functional\.EnumerableScanExtensions\.Scan\<T,TAcc\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, TAcc, System\.Func\<TAcc,T,TAcc\>\)\.TAcc')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')  
For seed `0`, `+` and `[1, 2, 3]`: `1, 3, 6` \(one value per element, so an empty
            source gives an empty sequence\)\. Lazy: nothing is enumerated until the result is iterated, and the source is
            enumerated once\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') source or accumulator records `result.null_source` or
            `result.null_delegate` in telemetry; it never throws\.