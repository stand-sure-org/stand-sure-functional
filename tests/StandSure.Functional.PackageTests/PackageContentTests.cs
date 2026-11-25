using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using AwesomeAssertions;

using Xunit;
using Xunit.OpenCategories.V3;

namespace StandSure.Functional.PackageTests;

/// <summary>
/// What consumers get inside the packed <c>.nupkg</c>: the DLL carries its portable PDB with every source file embedded,
/// so debuggers, IDEs and coding agents see the real code without a symbol server (the GitHub feed serves no .snupkg).
/// </summary>
[Category(nameof(PackageContentTests))]
public sealed class PackageContentTests
{
    private const string LibraryEntry = "lib/net11.0/StandSure.Functional.dll";

    // Portable PDB custom debug information kind for an embedded source document.
    private static readonly Guid EmbeddedSourceKind = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");

    [Fact]
    public void Package_HasNoSeparatePdb()
    {
        using var package = ZipFile.OpenRead(ConsumerProject.LatestPackage().FullName);

        package.Entries.Select(e => e.FullName).Should().Contain(LibraryEntry)
            .And.NotContain(name => name.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void PackagedDll_EmbedsItsPortablePdb()
    {
        using var pe = OpenPackagedLibrary();

        pe.ReadDebugDirectory().Should().Contain(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
    }

    [Theory]
    [InlineData("Unit.cs")]
    [InlineData("Result.T.Combinators.cs")]
    [InlineData("ResultUnitExtensions.cs")]
    public void PackagedPdb_EmbedsTheSourceOf(string fileName)
    {
        using var pe = OpenPackagedLibrary();
        using var provider = OpenEmbeddedPdb(pe);
        var pdb = provider.GetMetadataReader();
        var documents = Documents(pdb);

        var matches = documents.Where(d => Path.GetFileName(d.Name) == fileName).ToList();
        matches.Should().ContainSingle($"the PDB should list {fileName} once; documents: {string.Join(", ", documents.Select(d => d.Name))}");

        var hasEmbeddedSource = pdb.GetCustomDebugInformation(matches[0].Handle)
            .Select(pdb.GetCustomDebugInformation)
            .Any(info => pdb.GetGuid(info.Kind) == EmbeddedSourceKind);

        hasEmbeddedSource.Should().BeTrue($"{fileName} must be embedded so readers of the package see the real source");
    }

    // ContinuousIntegrationBuild maps source paths to /_/ so the package does not leak the runner's directory.
    [Fact]
    public void PackagedPdb_OnCi_UsesNormalisedSourcePaths()
    {
        Assert.SkipUnless(Environment.GetEnvironmentVariable("CI") == "true", "Path normalisation applies to CI builds only.");

        using var pe = OpenPackagedLibrary();
        using var provider = OpenEmbeddedPdb(pe);
        var documents = Documents(provider.GetMetadataReader());

        documents.Should().NotBeEmpty()
            .And.OnlyContain(d => d.Name.StartsWith("/_/", StringComparison.Ordinal));
    }

    private static PEReader OpenPackagedLibrary()
    {
        using var package = ZipFile.OpenRead(ConsumerProject.LatestPackage().FullName);
        var entry = package.GetEntry(LibraryEntry);
        entry.Should().NotBeNull($"the package should contain {LibraryEntry}");

        // PEReader needs a seekable stream; zip entry streams are not.
        var bytes = new MemoryStream();
        using (var stream = entry!.Open())
        {
            stream.CopyTo(bytes);
        }

        bytes.Position = 0;
        return new PEReader(bytes);
    }

    private static MetadataReaderProvider OpenEmbeddedPdb(PEReader pe)
    {
        var entries = pe.ReadDebugDirectory().Where(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb).ToList();
        entries.Should().ContainSingle("the packaged DLL should embed exactly one portable PDB");

        return pe.ReadEmbeddedPortablePdbDebugDirectoryData(entries[0]);
    }

    private static List<(DocumentHandle Handle, string Name)> Documents(MetadataReader pdb) =>
        [.. pdb.Documents.Select(h => (h, pdb.GetString(pdb.GetDocument(h).Name)))];
}
