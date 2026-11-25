#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[ResultUnitExtensions](index.md 'StandSure\.Functional\.ResultUnitExtensions')

## ResultUnitExtensions\.Bind\<TOut\>\(this Result\<Unit\>, Func\<Result\<TOut\>\>\) Method

Chains a step that needs no input after a step that produced nothing; a failure short\-circuits\.

```csharp
public static StandSure.Functional.Result<TOut> Bind<TOut>(this StandSure.Functional.Result<StandSure.Functional.Unit> result, System.Func<StandSure.Functional.Result<TOut>> bind);
```
#### Type parameters

<a name='StandSure.Functional.ResultUnitExtensions.Bind_TOut_(thisStandSure.Functional.Result_StandSure.Functional.Unit_,System.Func_StandSure.Functional.Result_TOut__).TOut'></a>

`TOut`

The value type of the chained result\.
#### Parameters

<a name='StandSure.Functional.ResultUnitExtensions.Bind_TOut_(thisStandSure.Functional.Result_StandSure.Functional.Unit_,System.Func_StandSure.Functional.Result_TOut__).result'></a>

`result` [StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[Unit](../Unit/index.md 'StandSure\.Functional\.Unit')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')

The result of the step that produced nothing\.

<a name='StandSure.Functional.ResultUnitExtensions.Bind_TOut_(thisStandSure.Functional.Result_StandSure.Functional.Unit_,System.Func_StandSure.Functional.Result_TOut__).bind'></a>

`bind` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](Bind_TOut_(thisResult_Unit_,Func_Result_TOut__).md#StandSure.Functional.ResultUnitExtensions.Bind_TOut_(thisStandSure.Functional.Result_StandSure.Functional.Unit_,System.Func_StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.ResultUnitExtensions\.Bind\<TOut\>\(this StandSure\.Functional\.Result\<StandSure\.Functional\.Unit\>, System\.Func\<StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

The next step\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Result&lt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')[TOut](Bind_TOut_(thisResult_Unit_,Func_Result_TOut__).md#StandSure.Functional.ResultUnitExtensions.Bind_TOut_(thisStandSure.Functional.Result_StandSure.Functional.Unit_,System.Func_StandSure.Functional.Result_TOut__).TOut 'StandSure\.Functional\.ResultUnitExtensions\.Bind\<TOut\>\(this StandSure\.Functional\.Result\<StandSure\.Functional\.Unit\>, System\.Func\<StandSure\.Functional\.Result\<TOut\>\>\)\.TOut')[&gt;](../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')  
The result of [bind](Bind_TOut_(thisResult_Unit_,Func_Result_TOut__).md#StandSure.Functional.ResultUnitExtensions.Bind_TOut_(thisStandSure.Functional.Result_StandSure.Functional.Unit_,System.Func_StandSure.Functional.Result_TOut__).bind 'StandSure\.Functional\.ResultUnitExtensions\.Bind\<TOut\>\(this StandSure\.Functional\.Result\<StandSure\.Functional\.Unit\>, System\.Func\<StandSure\.Functional\.Result\<TOut\>\>\)\.bind'), or the original failure without calling it\.