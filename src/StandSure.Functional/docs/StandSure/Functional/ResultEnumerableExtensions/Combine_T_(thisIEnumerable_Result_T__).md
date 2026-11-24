#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultEnumerableExtensions](index.md 'StandSure\.Functional\.ResultEnumerableExtensions')

## ResultEnumerableExtensions\.Combine\<T\>\(this IEnumerable\<Result\<T\>\>\) Method

Turns a sequence of results into one result holding every value, or every failure\.

```csharp
public static StandSure.Functional.Result<System.Collections.Generic.IReadOnlyList<T>> Combine<T>(this System.Collections.Generic.IEnumerable<StandSure.Functional.Result<T>>? source);
```
#### Type parameters

<a name='StandSure.Functional.ResultEnumerableExtensions.Combine_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).T'></a>

`T`
#### Parameters

<a name='StandSure.Functional.ResultEnumerableExtensions.Combine_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).source'></a>

`source` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](Combine_T_(thisIEnumerable_Result_T__).md#StandSure.Functional.ResultEnumerableExtensions.Combine_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).T 'StandSure\.Functional\.ResultEnumerableExtensions\.Combine\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[T](Combine_T_(thisIEnumerable_Result_T__).md#StandSure.Functional.ResultEnumerableExtensions.Combine_T_(thisSystem.Collections.Generic.IEnumerable_StandSure.Functional.Result_T__).T 'StandSure\.Functional\.ResultEnumerableExtensions\.Combine\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A success with the values in order \(empty for an empty sequence\), or a failure with code
            `result.combine_failed`, message "`n` of `m` failed" and \<b\>all\</b\> failures in
            [Errors](../Error/Errors.md 'StandSure\.Functional\.Error\.Errors')\. The failure's [Kind](../Error/Kind.md 'StandSure\.Functional\.Error\.Kind') is the children's kind when they all agree; for a mix it is
            [Unexpected](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unexpected 'StandSure\.Functional\.ErrorKind\.Unexpected') if any child is, else [Unavailable](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Unavailable 'StandSure\.Functional\.ErrorKind\.Unavailable') if any child is,
            else [Validation](../ErrorKind/index.md#StandSure.Functional.ErrorKind.Validation 'StandSure\.Functional\.ErrorKind\.Validation')\.