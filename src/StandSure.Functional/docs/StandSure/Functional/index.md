#### [StandSure\.Functional](../../index.md 'index')

## StandSure\.Functional Namespace

| Classes | |
| :--- | :--- |
| [EnumerablePairwiseExtensions](EnumerablePairwiseExtensions/index.md 'StandSure\.Functional\.EnumerablePairwiseExtensions') | The `Pairwise` sequence helper\. |
| [EnumerableScanExtensions](EnumerableScanExtensions/index.md 'StandSure\.Functional\.EnumerableScanExtensions') | The `Scan` sequence helper\. |
| [Error](Error/index.md 'StandSure\.Functional\.Error') | A failure: a kind, a stable machine\-readable code, a message and optional context\. |
| [None](None/index.md 'StandSure\.Functional\.None') | The absence case of an [Option&lt;T&gt;](Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')\. It has exactly one instance, [Instance](None/Instance.md 'StandSure\.Functional\.None\.Instance')\. |
| [Option](Option/index.md 'StandSure\.Functional\.Option') | Factory methods for [Option&lt;T&gt;](Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')\. |
| [OptionEnumerableExtensions](OptionEnumerableExtensions/index.md 'StandSure\.Functional\.OptionEnumerableExtensions') | Collection helpers for sequences of [Option&lt;T&gt;](Option_T_/index.md 'StandSure\.Functional\.Option\<T\>')\. |
| [Result](Result/index.md 'StandSure\.Functional\.Result') | Factory methods for [Result&lt;T&gt;](Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\. |
| [ResultEnumerableExtensions](ResultEnumerableExtensions/index.md 'StandSure\.Functional\.ResultEnumerableExtensions') | Collection helpers for sequences of [Result&lt;T&gt;](Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\. |
| [ResultEnumerableExtensions\.&lt;G&gt;$9062F940B5B80A9495FB44E7E97954A6&lt;$T0&gt;](ResultEnumerableExtensions/_G_$9062F940B5B80A9495FB44E7E97954A6_$T0_/index.md 'StandSure\.Functional\.ResultEnumerableExtensions\.\<G\>$9062F940B5B80A9495FB44E7E97954A6\<$T0\>') | |
| [ResultEnumerableExtensions\.&lt;G&gt;$9062F940B5B80A9495FB44E7E97954A6&lt;$T0&gt;\.&lt;M&gt;$B7BDD63E9074B607BD24BE1E6071887A](ResultEnumerableExtensions/_G_$9062F940B5B80A9495FB44E7E97954A6_$T0_/_M_$B7BDD63E9074B607BD24BE1E6071887A/index.md 'StandSure\.Functional\.ResultEnumerableExtensions\.\<G\>$9062F940B5B80A9495FB44E7E97954A6\<$T0\>\.\<M\>$B7BDD63E9074B607BD24BE1E6071887A') | |
| [ResultTaskExtensions](ResultTaskExtensions/index.md 'StandSure\.Functional\.ResultTaskExtensions') | Combinators for [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') of [Result&lt;T&gt;](Result_T_/index.md 'StandSure\.Functional\.Result\<T\>'), so a chain can mix awaited steps without intermediate `await` statements\. |
| [Try](Try/index.md 'StandSure\.Functional\.Try') | Bridges code that throws into [Result&lt;T&gt;](Result_T_/index.md 'StandSure\.Functional\.Result\<T\>')\. This is the only place in the library that catches exceptions\. |

| Structs | |
| :--- | :--- |
| [Option&lt;T&gt;](Option_T_/index.md 'StandSure\.Functional\.Option\<T\>') | An optional value: either some [T](Option_T_/index.md#StandSure.Functional.Option_T_.T 'StandSure\.Functional\.Option\<T\>\.T') or [None](None/index.md 'StandSure\.Functional\.None')\. |
| [Result&lt;T&gt;](Result_T_/index.md 'StandSure\.Functional\.Result\<T\>') | The outcome of an operation that can fail: either a success holding a [T](Result_T_/index.md#StandSure.Functional.Result_T_.T 'StandSure\.Functional\.Result\<T\>\.T') or a failure holding an [Error](Error/index.md 'StandSure\.Functional\.Error')\. |
| [Unit](Unit/index.md 'StandSure\.Functional\.Unit') | The type with exactly one value, used where a result carries no information \(the functional equivalent of [void](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/void 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/void')\)\. |

| Enums | |
| :--- | :--- |
| [ErrorKind](ErrorKind/index.md 'StandSure\.Functional\.ErrorKind') | The category of a failure, independent of its specific code\. |
