#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.Tap\(Action\<T\>\) Method

Runs a side effect on the success value and returns this result unchanged\.

```csharp
public StandSure.Functional.Result<T> Tap(System.Action<T> action);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.Tap(System.Action_T_).action'></a>

`action` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

The side effect\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value turns a success into a
            `result.null_delegate` failure\.

#### Returns
[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')  
This result\.