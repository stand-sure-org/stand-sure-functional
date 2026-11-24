#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option](index.md 'StandSure\.Functional\.Option')

## Option\.Some\<T\>\(T\) Method

Creates an option holding a value\.

```csharp
public static StandSure.Functional.Option<T> Some<T>(T? value);
```
#### Type parameters

<a name='StandSure.Functional.Option.Some_T_(T).T'></a>

`T`

The value type\.
#### Parameters

<a name='StandSure.Functional.Option.Some_T_(T).value'></a>

`value` [T](Some_T_(T).md#StandSure.Functional.Option.Some_T_(T).T 'StandSure\.Functional\.Option\.Some\<T\>\(T\)\.T')

The value\. A [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') value produces [None](../Option_T_/None.md 'StandSure\.Functional\.Option\<T\>\.None')\.

#### Returns
[StandSure\.Functional\.Option&lt;](../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')[T](Some_T_(T).md#StandSure.Functional.Option.Some_T_(T).T 'StandSure\.Functional\.Option\.Some\<T\>\(T\)\.T')[&gt;](../Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')  
An option that is some when [value](Some_T_(T).md#StandSure.Functional.Option.Some_T_(T).value 'StandSure\.Functional\.Option\.Some\<T\>\(T\)\.value') is not [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null'); otherwise none\.