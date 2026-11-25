#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultUnitExtensions](index.md 'StandSure\.Functional\.ResultUnitExtensions')

## ResultUnitExtensions\.Match\<T\>\(this Result\<T\>, Action\<Error\>, Action\<T\>\) Method

Runs one of two actions depending on the case\. The failure action comes first\.

```csharp
public static StandSure.Functional.Unit Match<T>(this StandSure.Functional.Result<T> result, System.Action<StandSure.Functional.Error> onFailure, System.Action<T> onSuccess);
```
#### Type parameters

<a name='StandSure.Functional.ResultUnitExtensions.Match_T_(thisStandSure.Functional.Result_T_,System.Action_StandSure.Functional.Error_,System.Action_T_).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.ResultUnitExtensions.Match_T_(thisStandSure.Functional.Result_T_,System.Action_StandSure.Functional.Error_,System.Action_T_).result'></a>

`result` [StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[T](Match_T_(thisResult_T_,Action_Error_,Action_T_).md#StandSure.Functional.ResultUnitExtensions.Match_T_(thisStandSure.Functional.Result_T_,System.Action_StandSure.Functional.Error_,System.Action_T_).T 'StandSure\.Functional\.ResultUnitExtensions\.Match\<T\>\(this StandSure\.Functional\.Result\<T\>, System\.Action\<StandSure\.Functional\.Error\>, System\.Action\<T\>\)\.T')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')

The result\.

<a name='StandSure.Functional.ResultUnitExtensions.Match_T_(thisStandSure.Functional.Result_T_,System.Action_StandSure.Functional.Error_,System.Action_T_).onFailure'></a>

`onFailure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Runs on a failure, including `default`, with the error\.

<a name='StandSure.Functional.ResultUnitExtensions.Match_T_(thisStandSure.Functional.Result_T_,System.Action_StandSure.Functional.Error_,System.Action_T_).onSuccess'></a>

`onSuccess` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[T](Match_T_(thisResult_T_,Action_Error_,Action_T_).md#StandSure.Functional.ResultUnitExtensions.Match_T_(thisStandSure.Functional.Result_T_,System.Action_StandSure.Functional.Error_,System.Action_T_).T 'StandSure\.Functional\.ResultUnitExtensions\.Match\<T\>\(this StandSure\.Functional\.Result\<T\>, System\.Action\<StandSure\.Functional\.Error\>, System\.Action\<T\>\)\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Runs on a success with the value\.

#### Returns
[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')  
[Value](../Unit/Value.md 'StandSure\.Functional\.Unit\.Value'), so the call also works as an expression\.

### Remarks
A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') action for the active branch runs nothing and records a
            `result.null_delegate` failure in telemetry\.