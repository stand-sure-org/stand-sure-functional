namespace ProbeRunner;

/// <summary>The single JSON line ProbeRunner writes to stdout.</summary>
/// <param name="Outcome"><c>returned</c>, <c>threw</c> or <c>crashed</c> (<c>timeout</c> is decided by the parent).</param>
/// <param name="ExceptionType">Full name of the exception the solution threw, when <paramref name="Outcome"/> is <c>threw</c>.</param>
/// <param name="Reason">Why the runner itself failed, when <paramref name="Outcome"/> is <c>crashed</c>.</param>
internal sealed record ProbeOutput(string Outcome, string? ExceptionType = null, string? Reason = null)
{
    public const string Returned = "returned";
    public const string Threw = "threw";
    public const string Crashed = "crashed";

    public static ProbeOutput Fail(string reason) => new(Crashed, Reason: reason);
}
