#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[OptionUnitExtensions](index.md 'StandSure\.Functional\.OptionUnitExtensions')

## OptionUnitExtensions\.Match\<T\>\(this Option\<T\>, Action, Action\<T\>\) Method

Runs one of two actions depending on the case\. The none action comes first\.

```csharp
public static StandSure.Functional.Unit Match<T>(this StandSure.Functional.Option<T> option, System.Action onNone, System.Action<T> onSome);
```
#### Type parameters

<a name='StandSure.Functional.OptionUnitExtensions.Match_T_(thisStandSure.Functional.Option_T_,System.Action,System.Action_T_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.OptionUnitExtensions.Match_T_(thisStandSure.Functional.Option_T_,System.Action,System.Action_T_).option'></a>

`option` [StandSure\.Functional\.Option&lt;](../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')[T](Match_T_(thisOption_T_,Action,Action_T_).md#StandSure.Functional.OptionUnitExtensions.Match_T_(thisStandSure.Functional.Option_T_,System.Action,System.Action_T_).T 'StandSure\.Functional\.OptionUnitExtensions\.Match\<T\>\(this StandSure\.Functional\.Option\<T\>, System\.Action, System\.Action\<T\>\)\.T')[&gt;](../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')

The option\.

<a name='StandSure.Functional.OptionUnitExtensions.Match_T_(thisStandSure.Functional.Option_T_,System.Action,System.Action_T_).onNone'></a>

`onNone` [System\.Action](https://learn.microsoft.com/en-us/dotnet/api/system.action 'System\.Action')

Runs on none, including `default`\.

<a name='StandSure.Functional.OptionUnitExtensions.Match_T_(thisStandSure.Functional.Option_T_,System.Action,System.Action_T_).onSome'></a>

`onSome` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[T](Match_T_(thisOption_T_,Action,Action_T_).md#StandSure.Functional.OptionUnitExtensions.Match_T_(thisStandSure.Functional.Option_T_,System.Action,System.Action_T_).T 'StandSure\.Functional\.OptionUnitExtensions\.Match\<T\>\(this StandSure\.Functional\.Option\<T\>, System\.Action, System\.Action\<T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Runs on some with the value\.

#### Returns
[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')  
[Value](../Unit/Value.md 'StandSure\.Functional\.Unit\.Value'), so the call also works as an expression\.

### Remarks
A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') action for the active branch runs nothing and records a
            `result.null_delegate` failure in telemetry\.