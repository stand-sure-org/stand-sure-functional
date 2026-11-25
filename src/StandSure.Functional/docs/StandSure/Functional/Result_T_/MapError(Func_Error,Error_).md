#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Result&lt;T&gt;](index.md 'StandSure\.Functional\.Result\<T\>')

## Result\<T\>\.MapError\(Func\<Error,Error\>\) Method

Transforms the error of a failure; a success passes through unchanged\.

```csharp
public StandSure.Functional.Result<T> MapError(System.Func<StandSure.Functional.Error,StandSure.Functional.Error> map);
```
#### Parameters

<a name='StandSure.Functional.Result_T_.MapError(System.Func_StandSure.Functional.Error,StandSure.Functional.Error_).map'></a>

`map` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[Error](../Error/index.md 'StandSure\.Functional\.Error')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The transformation\. It receives [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized') for `default`\.
            A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') return becomes [Uninitialized](../Error/Uninitialized.md 'StandSure\.Functional\.Error\.Uninitialized')\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') delegate
            keeps the original failure and records a `result.null_delegate` failure in telemetry\.

#### Returns
[StandSure\.Functional\.Result&lt;](index.md 'StandSure\.Functional\.Result\<T\>')[T](index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T')[&gt;](index.md 'StandSure\.Functional\.Result\<T\>')  
The original success, or a failure with the mapped error \(the original error when
            [map](MapError(Func_Error,Error_).md#StandSure.Functional.Result_T_.MapError(System.Func_StandSure.Functional.Error,StandSure.Functional.Error_).map 'StandSure\.Functional\.Result\<T\>\.MapError\(System\.Func\<StandSure\.Functional\.Error,StandSure\.Functional\.Error\>\)\.map') is [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\)\.