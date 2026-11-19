namespace ModelHarness.Tasks;

/// <summary>One harness task, loaded from <c>harness/tasks/NN-slug/</c>.</summary>
/// <param name="Id">The task directory name, e.g. <c>01-create-user</c>.</param>
/// <param name="Prompt">The complete user prompt: <c>task.md</c> without its front matter, with the fixture source fenced in place of the fixture marker.</param>
/// <param name="FixtureSource">The contents of <c>Fixture.cs</c>.</param>
/// <param name="ProbeSource">The contents of <c>Probe.cs</c>.</param>
/// <param name="EntryType">The class the model writes; always <c>Solution</c>.</param>
/// <param name="EntryMethod">The public instance method the probe invokes.</param>
public sealed record HarnessTask(string Id, string Prompt, string FixtureSource, string ProbeSource, string EntryType, string EntryMethod);
