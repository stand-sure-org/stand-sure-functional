using System;
using System.IO;
using System.Runtime.CompilerServices;

using PublicApiGenerator;

using Xunit;

namespace StandSure.Functional.ApprovalTests;

public sealed class PublicApiTests
{
    [Fact]
    public void PublicApi_HasNotChanged()
    {
        var actual = Normalize(typeof(Result).Assembly.GeneratePublicApi());
        var verifiedPath = Path.Combine(SourceDirectory(), "PublicApi.verified.txt");
        var receivedPath = Path.Combine(SourceDirectory(), "PublicApi.received.txt");

        var expected = File.Exists(verifiedPath) ? Normalize(File.ReadAllText(verifiedPath)) : null;

        if (expected == actual)
        {
            File.Delete(receivedPath);
            return;
        }

        File.WriteAllText(receivedPath, actual);

        Assert.Fail(
            $"The public API differs from the approved snapshot. Review '{receivedPath}' against '{verifiedPath}'; "
            + "if the change is intended, replace the verified file with the received file.");
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Trim() + "\n";

    // Source directory is stable under `dotnet test` from any working directory and lets the received file land beside the verified one.
    private static string SourceDirectory([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;
}
