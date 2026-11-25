#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## ResultUnitExtensions Class

Side\-effect forms of [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>') combinators\. `void` has no place in a chain of results, so these
adapt [System\.Action&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1') and parameterless steps to [Unit](../Unit/index.md 'StandSure\.Functional\.Unit') and forward to the ordinary combinator\.

```csharp
public static class ResultUnitExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → ResultUnitExtensions

### Remarks
Each method forwards to the [Result&lt;T&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>') method of the same name, which starts the span and applies the
usual [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\-delegate rules\. A lambda that returns a value binds to the ordinary overload instead\.

| Methods | |
| :--- | :--- |
| [Bind&lt;TOut&gt;\(this Result&lt;Unit&gt;, Func&lt;Result&lt;TOut&gt;&gt;\)](Bind_TOut_(thisResult_Unit_,Func_Result_TOut__).md 'StandSure\.Functional\.ResultUnitExtensions\.Bind\<TOut\>\(this StandSure\.Functional\.Result\<StandSure\.Functional\.Unit\>, System\.Func\<StandSure\.Functional\.Result\<TOut\>\>\)') | Chains a step that needs no input after a step that produced nothing; a failure short\-circuits\. |
| [Match&lt;T&gt;\(this Result&lt;T&gt;, Action&lt;Error&gt;, Action&lt;T&gt;\)](Match_T_(thisResult_T_,Action_Error_,Action_T_).md 'StandSure\.Functional\.ResultUnitExtensions\.Match\<T\>\(this StandSure\.Functional\.Result\<T\>, System\.Action\<StandSure\.Functional\.Error\>, System\.Action\<T\>\)') | Runs one of two actions depending on the case\. The failure action comes first\. |
