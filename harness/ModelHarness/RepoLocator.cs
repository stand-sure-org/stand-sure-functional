namespace ModelHarness;

using System;
using System.IO;

/// <summary>Finds the repository root by walking up to the directory holding <c>StandSure.Functional.slnx</c>.</summary>
public static class RepoLocator
{
    /// <summary>The marker file that identifies the repository root.</summary>
    public const string SolutionFileName = "StandSure.Functional.slnx";

    /// <summary>Walks up from <paramref name="startDirectory"/> (usually <see cref="AppContext.BaseDirectory"/>) to the repository root.</summary>
    /// <exception cref="DirectoryNotFoundException">No ancestor contains the solution file.</exception>
    public static string FindRoot(string startDirectory)
    {
        for (var dir = new DirectoryInfo(startDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, SolutionFileName)))
            {
                return dir.FullName;
            }
        }

        throw new DirectoryNotFoundException($"No ancestor of '{startDirectory}' contains {SolutionFileName}.");
    }
}
