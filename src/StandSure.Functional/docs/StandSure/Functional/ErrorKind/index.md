#### [StandSure\.Functional](../../../index.md 'index')
### [StandSure\.Functional](../index.md 'StandSure\.Functional')

## ErrorKind Enum

The category of a failure, independent of its specific code\.

```csharp
public enum ErrorKind
```
### Fields

<a name='StandSure.Functional.ErrorKind.Validation'></a>

`Validation` 0

The input was invalid\.

<a name='StandSure.Functional.ErrorKind.NotFound'></a>

`NotFound` 1

The requested thing does not exist\.

<a name='StandSure.Functional.ErrorKind.Conflict'></a>

`Conflict` 2

The request conflicts with current state\.

<a name='StandSure.Functional.ErrorKind.Unauthorized'></a>

`Unauthorized` 3

The caller is not authenticated\.

<a name='StandSure.Functional.ErrorKind.Forbidden'></a>

`Forbidden` 4

The caller is authenticated but not allowed\.

<a name='StandSure.Functional.ErrorKind.Unavailable'></a>

`Unavailable` 5

A dependency is temporarily unavailable\.

<a name='StandSure.Functional.ErrorKind.Unexpected'></a>

`Unexpected` 6

An unexpected failure\.