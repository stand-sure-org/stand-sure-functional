namespace ModelHarness.Tasks;

using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Loads the harness task set from <c>harness/tasks/</c>.
/// </summary>
/// <remarks>
/// <para>Layout: one directory per task, named <c>NN-slug</c>, holding three files.</para>
/// <list type="bullet">
/// <item><c>task.md</c>: front matter (<c>entry-method: Name</c> between <c>---</c> lines), then the
/// prompt body. The body must contain <see cref="FixtureMarker"/> exactly once.</item>
/// <item><c>Fixture.cs</c>: the caller-defined dependency interfaces and DTOs the model codes against.
/// Global namespace, no reference to StandSure.Functional, so it compiles in both conditions.</item>
/// <item><c>Probe.cs</c>: the failure-injecting stub plus <c>public static class HarnessProbe</c> with
/// <c>object?[] ConstructorArguments()</c> and <c>object?[] Arguments()</c>. Global namespace; it
/// references only the fixture, never <c>Solution</c>.</item>
/// </list>
/// <para>Prompt assembly: <see cref="HarnessTask.Prompt"/> is the complete user message. It is the
/// <c>task.md</c> body without front matter, with <see cref="FixtureMarker"/> replaced by the fixture
/// source in a <c>```csharp</c> fence, and with line endings normalised to <c>\n</c> so the prompt hash
/// is stable. The runner sends it unchanged.</para>
/// <para>Invocation convention (every task): the model writes a public class <c>Solution</c> whose public
/// constructor takes the task's dependencies, and a public instance method <see cref="HarnessTask.EntryMethod"/>.
/// The probe locates <c>Solution</c> by simple name in any namespace, constructs it with
/// <c>HarnessProbe.ConstructorArguments()</c>, invokes the entry method with <c>HarnessProbe.Arguments()</c>,
/// and awaits the returned value when it is a <c>Task</c> or <c>ValueTask</c>.</para>
/// </remarks>
public static class TaskCatalog
{
    /// <summary>The placeholder in <c>task.md</c> replaced by the fenced fixture source.</summary>
    public const string FixtureMarker = "{{FIXTURE}}";

    /// <summary>The class name every task asks the model to write.</summary>
    public const string EntryTypeName = "Solution";

    private const string FrontMatterDelimiter = "---";
    private const string EntryMethodKey = "entry-method:";

    /// <summary>Loads every task under <paramref name="root"/>, ordered by directory name (ordinal).</summary>
    /// <param name="root">The tasks directory itself (<c>harness/tasks</c>), not the repository root.</param>
    /// <exception cref="InvalidDataException">A task directory is missing a file or its <c>task.md</c> is malformed.</exception>
    public static IReadOnlyList<HarnessTask> Load(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        var directories = Directory.GetDirectories(root);
        Array.Sort(directories, StringComparer.Ordinal);

        var tasks = new List<HarnessTask>(directories.Length);

        foreach (var directory in directories)
        {
            tasks.Add(LoadOne(directory));
        }

        return tasks;
    }

    private static HarnessTask LoadOne(string directory)
    {
        var id = Path.GetFileName(directory);
        var markdown = ReadRequired(directory, "task.md");
        var fixture = ReadRequired(directory, "Fixture.cs");
        var probe = ReadRequired(directory, "Probe.cs");

        var (entryMethod, body) = SplitFrontMatter(id, markdown);

        if (CountOccurrences(body, FixtureMarker) != 1)
        {
            throw new InvalidDataException($"{id}/task.md must contain {FixtureMarker} exactly once.");
        }

        var fenced = "```csharp\n" + fixture.TrimEnd('\n') + "\n```";
        var prompt = body.Replace(FixtureMarker, fenced, StringComparison.Ordinal).Trim() + "\n";

        return new HarnessTask(id, prompt, fixture, probe, EntryTypeName, entryMethod);
    }

    private static string ReadRequired(string directory, string fileName)
    {
        var path = Path.Combine(directory, fileName);

        if (!File.Exists(path))
        {
            throw new InvalidDataException($"Task file missing: {path}");
        }

        return File.ReadAllText(path).Replace("\r\n", "\n", StringComparison.Ordinal);
    }

    private static (string EntryMethod, string Body) SplitFrontMatter(string id, string markdown)
    {
        var lines = markdown.Split('\n');

        if (lines.Length == 0 || lines[0].Trim() != FrontMatterDelimiter)
        {
            throw new InvalidDataException($"{id}/task.md must start with front matter.");
        }

        string? entryMethod = null;

        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (line == FrontMatterDelimiter)
            {
                if (string.IsNullOrEmpty(entryMethod))
                {
                    throw new InvalidDataException($"{id}/task.md front matter has no '{EntryMethodKey}'.");
                }

                return (entryMethod, string.Join('\n', lines[(i + 1)..]));
            }

            if (line.StartsWith(EntryMethodKey, StringComparison.Ordinal))
            {
                entryMethod = line[EntryMethodKey.Length..].Trim();
            }
        }

        throw new InvalidDataException($"{id}/task.md front matter is not closed.");
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;

        for (var index = text.IndexOf(value, StringComparison.Ordinal); index >= 0; index = text.IndexOf(value, index + value.Length, StringComparison.Ordinal))
        {
            count++;
        }

        return count;
    }
}
