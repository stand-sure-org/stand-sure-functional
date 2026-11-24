#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultEnumerableExtensions](index.md 'StandSure\.Functional\.ResultEnumerableExtensions')

## ResultEnumerableExtensions\.Traverse\<T,TOut\>\(this IEnumerable\<T\>, Func\<T,Result\<TOut\>\>\) Method

Maps every element to a result, then combines them as [Combine&lt;T&gt;\(this IEnumerable&lt;Result&lt;T&gt;&gt;\)](Combine_T_(thisIEnumerable_Result_T__).md 'StandSure\.Functional\.ResultEnumerableExtensions\.Combine\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)') does\.

```csharp
public static StandSure.Functional.Result<System.Collections.Generic.IReadOnlyList<TOut>> Traverse<T,TOut>(this System.Collections.Generic.IEnumerable<T>? source, System.Func<T,StandSure.Functional.Result<TOut>> map);
```
#### Type parameters

<a name='StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).T'></a>

`T`

The element type\.

<a name='StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).TOut'></a>

`TOut`

The mapped value type\.
#### Parameters

<a name='StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).source'></a>

`source` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[T](Traverse_T,TOut_(thisIEnumerable_T_,Func_T,Result_TOut__).md#StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).T 'StandSure\.Functional\.ResultEnumerableExtensions\.Traverse\<T,TOut\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The elements\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives a `result.null_source` failure\.

<a name='StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[T](Traverse_T,TOut_(thisIEnumerable_T_,Func_T,Result_TOut__).md#StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).T 'StandSure\.Functional\.ResultEnumerableExtensions\.Traverse\<T,TOut\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.T')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](Traverse_T,TOut_(thisIEnumerable_T_,Func_T,Result_TOut__).md#StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.ResultEnumerableExtensions\.Traverse\<T,TOut\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The mapper\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value gives a `result.null_delegate` failure\.
            It runs for every element: this helper does not stop at the first failure, so side effects of the mapper
            happen for all elements\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[System\.Collections\.Generic\.IReadOnlyList&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[TOut](Traverse_T,TOut_(thisIEnumerable_T_,Func_T,Result_TOut__).md#StandSure.Functional.ResultEnumerableExtensions.Traverse_T,TOut_(thisSystem.Collections.Generic.IEnumerable_T_,System.Func_T,StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.ResultEnumerableExtensions\.Traverse\<T,TOut\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1 'System\.Collections\.Generic\.IReadOnlyList\`1')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
A success with every mapped value in order, or a failure holding all failures, with its kind chosen as
            for `Combine`\.