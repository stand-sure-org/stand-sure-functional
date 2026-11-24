#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional').[Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>')

## Option\<T\> Constructors

| Overloads | |
| :--- | :--- |
| [Option\(None\)](Option_T_.md#StandSure.Functional.Option_T_.Option(StandSure.Functional.None) 'StandSure\.Functional\.Option\<T\>\.Option\(StandSure\.Functional\.None\)') | Initializes a new instance of the [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>') struct as none\. |
| [Option\(T\)](Option_T_.md#StandSure.Functional.Option_T_.Option(T) 'StandSure\.Functional\.Option\<T\>\.Option\(T\)') | Initializes a new instance of the [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>') struct holding a value\. |

<a name='StandSure.Functional.Option_T_.Option(StandSure.Functional.None)'></a>

## Option\(None\) Constructor

Initializes a new instance of the [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>') struct as none\.

```csharp
public Option(StandSure.Functional.None none);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.Option(StandSure.Functional.None).none'></a>

`none` [None](../None/index.md 'StandSure\.Functional\.None')

Ignored; the only [None](../None/index.md 'StandSure\.Functional\.None') value is the absence case\.

<a name='StandSure.Functional.Option_T_.Option(T)'></a>

## Option\(T\) Constructor

Initializes a new instance of the [Option&lt;T&gt;](index.md 'StandSure\.Functional\.Option\<T\>') struct holding a value\.

```csharp
public Option(T? value);
```
#### Parameters

<a name='StandSure.Functional.Option_T_.Option(T).value'></a>

`value` [T](index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T')

The value; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') produces none\.