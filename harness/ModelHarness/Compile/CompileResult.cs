namespace ModelHarness.Compile;

using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;

/// <summary>The outcome of <see cref="SolutionCompiler.Compile"/>.</summary>
/// <param name="Success">True when the compilation emitted with no errors (warnings do not count).</param>
/// <param name="Diagnostics">Errors only, formatted <c>id: message [file(line,col)]</c>.</param>
/// <param name="Assembly">The emitted assembly image, or <see langword="null"/> on failure.</param>
/// <param name="Compilation">The compilation (model + fixture + probe), including warnings such as CS8509, for later analysis.</param>
/// <param name="StrippedTypes">Names of model-declared top-level types removed because the fixture or probe declares them.</param>
public sealed record CompileResult(
    bool Success,
    IReadOnlyList<string> Diagnostics,
    byte[]? Assembly,
    CSharpCompilation Compilation,
    IReadOnlyList<string> StrippedTypes);
