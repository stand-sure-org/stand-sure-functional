#### [StandSure\.Functional](../../../../index.md 'index')
### [StandSure\.Functional\.Json](../index.md 'StandSure\.Functional\.Json').[FunctionalJsonOptionsExtensions](index.md 'StandSure\.Functional\.Json\.FunctionalJsonOptionsExtensions')

## FunctionalJsonOptionsExtensions\.AddFunctional\(this JsonSerializerOptions\) Method

Registers the wire format for [Error](../../Error/index.md 'StandSure\.Functional\.Error') on the options\.

```csharp
public static System.Text.Json.JsonSerializerOptions AddFunctional(this System.Text.Json.JsonSerializerOptions options);
```
#### Parameters

<a name='StandSure.Functional.Json.FunctionalJsonOptionsExtensions.AddFunctional(thisSystem.Text.Json.JsonSerializerOptions).options'></a>

`options` [System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')

The options to extend\. Must not yet be in use \(read\-only\)\.

#### Returns
[System\.Text\.Json\.JsonSerializerOptions](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializeroptions 'System\.Text\.Json\.JsonSerializerOptions')  
The same [options](AddFunctional(thisJsonSerializerOptions).md#StandSure.Functional.Json.FunctionalJsonOptionsExtensions.AddFunctional(thisSystem.Text.Json.JsonSerializerOptions).options 'StandSure\.Functional\.Json\.FunctionalJsonOptionsExtensions\.AddFunctional\(this System\.Text\.Json\.JsonSerializerOptions\)\.options'), for chaining\.

### Remarks
[Result&lt;T&gt;](../../Result_T_/index.md 'StandSure\.Functional\.Result\<T\>') and [Option&lt;T&gt;](../../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>') already carry their converters and serialize
            [Error](../../Error/index.md 'StandSure\.Functional\.Error') without this call\. Call it so that [Error](../../Error/index.md 'StandSure\.Functional\.Error') also serializes in the wire format when
            used directly\. The success value type `T` is resolved through [options](AddFunctional(thisJsonSerializerOptions).md#StandSure.Functional.Json.FunctionalJsonOptionsExtensions.AddFunctional(thisSystem.Text.Json.JsonSerializerOptions).options 'StandSure\.Functional\.Json\.FunctionalJsonOptionsExtensions\.AddFunctional\(this System\.Text\.Json\.JsonSerializerOptions\)\.options'): on options with no
            resolver, System\.Text\.Json's default applies as usual; when the caller has set one \(for example a
            source\-generated context in a native AOT app\), the library's metadata is placed ahead of it\.