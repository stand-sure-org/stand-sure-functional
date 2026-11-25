namespace StandSure.Functional.PackageTests;

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A throwaway consumer project in a temp directory that references the packed StandSure.Functional package from the local
/// feed (<c>artifacts/packages</c>), restored into its own package cache so a stale <c>~/.nuget/packages</c> copy is never used.
/// </summary>
public sealed class ConsumerProject : IDisposable
{
    private static readonly TimeSpan BuildTimeout = TimeSpan.FromMinutes(4);

    private ConsumerProject(string directory)
    {
        this.Directory = directory;
    }

    /// <summary>Gets the consumer project directory.</summary>
    public string Directory { get; }

    /// <summary>Writes a class library with <paramref name="source"/> as its only file, implicit usings off.</summary>
    public static ConsumerProject Create(string source)
    {
        var feed = Metadata("PackageFeed");
        var packageId = Metadata("PackageId");
        var version = LatestVersion(feed, packageId);
        var directory = Path.Combine(Path.GetTempPath(), "standsure-pkgtest-" + Guid.NewGuid().ToString("N"));
        System.IO.Directory.CreateDirectory(directory);

        File.Copy(Path.Combine(Metadata("RepoRoot"), "global.json"), Path.Combine(directory, "global.json"));

        File.WriteAllText(Path.Combine(directory, "nuget.config"),
            $"""
             <?xml version="1.0" encoding="utf-8"?>
             <configuration>
               <packageSources>
                 <clear />
                 <add key="local" value="{feed}" />
               </packageSources>
             </configuration>
             """);

        File.WriteAllText(Path.Combine(directory, "Consumer.csproj"),
            $"""
             <Project Sdk="Microsoft.NET.Sdk">
               <PropertyGroup>
                 <TargetFramework>net11.0</TargetFramework>
                 <Nullable>enable</Nullable>
                 <ImplicitUsings>disable</ImplicitUsings>
                 <RestorePackagesPath>{Path.Combine(directory, "packages")}</RestorePackagesPath>
               </PropertyGroup>
               <ItemGroup>
                 <PackageReference Include="{packageId}" Version="[{version}]" />
               </ItemGroup>
             </Project>
             """);

        File.WriteAllText(Path.Combine(directory, "Consumer.cs"), source);
        return new ConsumerProject(directory);
    }

    /// <summary>Runs <c>dotnet build</c> on the consumer (no node reuse, no shared compiler, so nothing outlives the test).</summary>
    public async Task<BuildResult> BuildAsync(CancellationToken cancellationToken)
    {
        var start = new ProcessStartInfo(DotnetPath())
        {
            WorkingDirectory = this.Directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        foreach (var arg in new[] { "build", "--nologo", "-nodeReuse:false", "-p:UseSharedCompilation=false", "-v:minimal" })
        {
            start.ArgumentList.Add(arg);
        }

        // The test host may inherit MSBuild variables from `dotnet test`; they would point the child build at the parent's MSBuild.
        foreach (var key in start.Environment.Keys.Where(k => k.StartsWith("MSBUILD", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            start.Environment.Remove(key);
        }

        start.Environment["MSBUILDDISABLENODEREUSE"] = "1";
        start.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";

        using var process = Process.Start(start) ?? throw new InvalidOperationException("dotnet did not start.");
        var stdout = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderr = process.StandardError.ReadToEndAsync(cancellationToken);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(BuildTimeout);

        try
        {
            await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }

        var output = new StringBuilder()
            .Append(await stdout.ConfigureAwait(false))
            .Append(await stderr.ConfigureAwait(false))
            .ToString();

        return new BuildResult(process.ExitCode, output);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            System.IO.Directory.Delete(this.Directory, recursive: true);
        }
        catch (IOException)
        {
            // Best effort: a leftover temp directory is harmless.
        }
        catch (UnauthorizedAccessException)
        {
            // Best effort: a leftover temp directory is harmless.
        }
    }

    private static string Metadata(string key) =>
        typeof(ConsumerProject).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(a => a.Key == key)
            .Value ?? throw new InvalidOperationException($"Assembly metadata '{key}' is empty.");

    private static string LatestVersion(string feed, string packageId)
    {
        var prefix = packageId + ".";

        var package = new DirectoryInfo(feed)
                          .EnumerateFiles(prefix + "*.nupkg")
                          .Where(f => !f.Name.EndsWith(".symbols.nupkg", StringComparison.OrdinalIgnoreCase))
                          .OrderByDescending(f => f.LastWriteTimeUtc)
                          .FirstOrDefault()
                      ?? throw new FileNotFoundException($"No {packageId} package in {feed}. Build src/{packageId} first; it packs on build.");

        return package.Name[prefix.Length..^".nupkg".Length];
    }


    /// <summary>The newest packed <c>.nupkg</c> of the library in the local feed, the file consumers restore.</summary>
    internal static FileInfo LatestPackage()
    {
        var feed = Metadata("PackageFeed");
        var packageId = Metadata("PackageId");

        return new FileInfo(Path.Combine(feed, $"{packageId}.{LatestVersion(feed, packageId)}.nupkg"));
    }

    private static string DotnetPath()
    {
        var host = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
        return string.IsNullOrEmpty(host) ? "dotnet" : host;
    }
}
