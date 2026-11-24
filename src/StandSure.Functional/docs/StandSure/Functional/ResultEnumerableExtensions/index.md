#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## ResultEnumerableExtensions Class

Collection helpers for sequences of [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\.

```csharp
public static class ResultEnumerableExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → ResultEnumerableExtensions

### Remarks
These helpers never throw\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') sequence yields a `result.null_source` failure \(or empty
lists for [Partition&lt;T&gt;\(this IEnumerable&lt;Result&lt;T&gt;&gt;\)](Partition_T_(thisIEnumerable_Result_T__).md 'StandSure\.Functional\.ResultEnumerableExtensions\.Partition\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)')\), and a [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') delegate a `result.null_delegate` failure;
both are recorded in telemetry\. A `default(Result<T>)` element counts as a failure with
[Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\. A delegate that throws is not caught\. All three helpers enumerate the
sequence once, eagerly\.

| Methods | |
| :--- | :--- |
| [Combine&lt;T&gt;\(this IEnumerable&lt;Result&lt;T&gt;&gt;\)](Combine_T_(thisIEnumerable_Result_T__).md 'StandSure\.Functional\.ResultEnumerableExtensions\.Combine\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)') | Turns a sequence of results into one result holding every value, or every failure\. |
| [Partition&lt;T&gt;\(this IEnumerable&lt;Result&lt;T&gt;&gt;\)](Partition_T_(thisIEnumerable_Result_T__).md 'StandSure\.Functional\.ResultEnumerableExtensions\.Partition\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)') | Separates the successes from the failures, keeping the order of each\. |
| [Traverse&lt;T,TOut&gt;\(this IEnumerable&lt;T&gt;, Func&lt;T,Result&lt;TOut&gt;&gt;\)](Traverse_T,TOut_(thisIEnumerable_T_,Func_T,Result_TOut__).md 'StandSure\.Functional\.ResultEnumerableExtensions\.Traverse\<T,TOut\>\(this System\.Collections\.Generic\.IEnumerable\<T\>, System\.Func\<T,StandSure\.Functional\.Result\<TOut\>\>\)') | Maps every element to a result, then combines them as [Combine&lt;T&gt;\(this IEnumerable&lt;Result&lt;T&gt;&gt;\)](Combine_T_(thisIEnumerable_Result_T__).md 'StandSure\.Functional\.ResultEnumerableExtensions\.Combine\<T\>\(this System\.Collections\.Generic\.IEnumerable\<StandSure\.Functional\.Result\<T\>\>\)') does\. |
