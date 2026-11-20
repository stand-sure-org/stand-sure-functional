namespace ModelHarness.Compilation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// Locates the net11.0 reference assemblies (shared framework plus ASP.NET Core) under the dotnet root,
/// for compiling fixtures and model output with Roslyn.
/// </summary>
/// <remarks>
/// The dotnet root is <c>DOTNET_ROOT</c> when set and it contains <c>packs/</c>, otherwise it is derived
/// from the running runtime (<c>{root}/shared/Microsoft.NETCore.App/{version}/</c>). The highest pack
/// version whose major matches the running runtime is used. Assemblies are unique by file name; the
/// shared-framework pack wins over the ASP.NET Core pack.
/// </remarks>
public static class ReferenceAssemblies
{
    private const string NetCorePack = "Microsoft.NETCore.App.Ref";
    private const string AspNetCorePack = "Microsoft.AspNetCore.App.Ref";

    private static readonly Lazy<IReadOnlyList<string>> Cached = new(Resolve);

    /// <summary>Full paths of every reference assembly: <c>Microsoft.NETCore.App.Ref</c> then <c>Microsoft.AspNetCore.App.Ref</c>.</summary>
    /// <exception cref="DirectoryNotFoundException">A reference pack for the running major version is not installed.</exception>
    public static IReadOnlyList<string> All() => Cached.Value;

    /// <summary>The dotnet installation root (the directory holding <c>packs/</c> and <c>shared/</c>).</summary>
    public static string DotnetRoot()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrEmpty(fromEnvironment) && Directory.Exists(Path.Combine(fromEnvironment, "packs")))
        {
            return fromEnvironment;
        }

        // RuntimeEnvironment.GetRuntimeDirectory() = {root}/shared/Microsoft.NETCore.App/{version}/
        var runtimeDirectory = new DirectoryInfo(RuntimeEnvironment.GetRuntimeDirectory().TrimEnd(Path.DirectorySeparatorChar));
        var root = runtimeDirectory.Parent?.Parent?.Parent
            ?? throw new DirectoryNotFoundException($"Cannot derive the dotnet root from '{runtimeDirectory.FullName}'.");
        return root.FullName;
    }

    private static IReadOnlyList<string> Resolve()
    {
        var root = DotnetRoot();
        var major = Environment.Version.Major;
        var byName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pack in new[] { NetCorePack, AspNetCorePack })
        {
            foreach (var path in PackAssemblies(root, pack, major))
            {
                byName.TryAdd(Path.GetFileName(path), path);
            }
        }

        return [.. byName.Values.Order(StringComparer.Ordinal)];
    }

    private static string[] PackAssemblies(string root, string pack, int major)
    {
        var packDirectory = Path.Combine(root, "packs", pack);
        var tfm = $"net{major}.0";
        var versionDirectory = Directory.Exists(packDirectory)
            ? Directory.GetDirectories(packDirectory)
                .Select(d => (Path: d, Version: Version.TryParse(Path.GetFileName(d).Split('-')[0], out var v) ? v : null))
                .Where(d => d.Version is not null && d.Version.Major == major && Directory.Exists(Path.Combine(d.Path, "ref", tfm)))
                .OrderByDescending(d => d.Version)
                .Select(d => d.Path)
                .FirstOrDefault()
            : null;

        if (versionDirectory is null)
        {
            throw new DirectoryNotFoundException($"No {pack} {major}.x reference pack with ref/{tfm} under '{packDirectory}'.");
        }

        return Directory.GetFiles(Path.Combine(versionDirectory, "ref", tfm), "*.dll");
    }
}
