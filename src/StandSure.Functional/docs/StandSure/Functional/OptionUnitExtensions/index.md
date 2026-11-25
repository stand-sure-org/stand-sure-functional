#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## OptionUnitExtensions Class

Side\-effect forms of [Option&lt;T&gt;](../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>') combinators\. They adapt actions to [Unit](../Unit/index.md 'StandSure\.Functional\.Unit') and forward to
the ordinary combinator, which starts the span and applies the usual [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\-delegate rules\.

```csharp
public static class OptionUnitExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → OptionUnitExtensions

| Methods | |
| :--- | :--- |
| [Match&lt;T&gt;\(this Option&lt;T&gt;, Action, Action&lt;T&gt;\)](Match_T_(thisOption_T_,Action,Action_T_).md 'StandSure\.Functional\.OptionUnitExtensions\.Match\<T\>\(this StandSure\.Functional\.Option\<T\>, System\.Action, System\.Action\<T\>\)') | Runs one of two actions depending on the case\. The none action comes first\. |
