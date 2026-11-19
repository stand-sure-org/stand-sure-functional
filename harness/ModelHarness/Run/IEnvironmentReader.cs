namespace ModelHarness.Run;

/// <summary>Reads environment variables; a seam so key lookup can be tested.</summary>
public interface IEnvironmentReader
{
    string? Get(string name);
}
