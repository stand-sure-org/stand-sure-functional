namespace ModelHarness.Probe;

using System.Text.Json.Serialization;

/// <summary>How a single behavioural probe of a compiled solution ended.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<ProbeOutcome>))]
public enum ProbeOutcome
{
    /// <summary>The entry method returned (and its Task-like result, if any, completed) without throwing.</summary>
    [JsonStringEnumMemberName("returned")]
    Returned,

    /// <summary>The solution's constructor or entry method threw; <see cref="ProbeResult.ExceptionType"/> names the exception.</summary>
    [JsonStringEnumMemberName("threw")]
    Threw,

    /// <summary>The probe ran past its timeout and its process tree was killed.</summary>
    [JsonStringEnumMemberName("timeout")]
    Timeout,

    /// <summary>No verdict: the process died without a result line, or the runner itself failed; <see cref="ProbeResult.Reason"/> says why.</summary>
    [JsonStringEnumMemberName("crashed")]
    Crashed,
}
