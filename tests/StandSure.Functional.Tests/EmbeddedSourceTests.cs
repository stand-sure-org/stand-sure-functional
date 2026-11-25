using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// The library ships its PDB inside the DLL with every source file embedded, so debuggers, IDEs and coding agents
/// that read the package see the real code and doc comments without a symbol server (the GitHub feed serves none).
/// </summary>
public sealed class EmbeddedSourceTests
{
    // Portable PDB custom debug information kind for an embedded source document.
    private static readonly Guid EmbeddedSourceKind = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");

    [Fact]
    public void LibraryDll_EmbedsItsPortablePdb()
    {
        using var pe = OpenLibrary();

        pe.ReadDebugDirectory().Should().Contain(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
    }

    [Theory]
    [InlineData("Unit.cs")]
    [InlineData("Result.T.Combinators.cs")]
    [InlineData("ResultUnitExtensions.cs")]
    public void EmbeddedPdb_ContainsTheSourceOf(string fileName)
    {
        using var pe = OpenLibrary();
        var entry = pe.ReadDebugDirectory().Single(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
        using var provider = pe.ReadEmbeddedPortablePdbDebugDirectoryData(entry);
        var pdb = provider.GetMetadataReader();

        var document = pdb.Documents
            .Select(h => (Handle: h, Name: pdb.GetString(pdb.GetDocument(h).Name)))
            .Single(d => Path.GetFileName(d.Name) == fileName);

        var hasEmbeddedSource = pdb.GetCustomDebugInformation(document.Handle)
            .Select(pdb.GetCustomDebugInformation)
            .Any(info => pdb.GetGuid(info.Kind) == EmbeddedSourceKind);

        hasEmbeddedSource.Should().BeTrue($"{fileName} must be embedded so readers of the package see the real source");
    }

    private static PEReader OpenLibrary() => new(File.OpenRead(typeof(Unit).Assembly.Location));
}
