namespace ModelHarness.Compile;

/// <summary>How <see cref="CodeExtractor"/> obtained the code.</summary>
public enum ExtractStatus
{
    /// <summary>One C# fence, or no fence and the raw text parsed as C#.</summary>
    Single,

    /// <summary>Several C# fences were joined in order.</summary>
    Concatenated,

    /// <summary>
    /// A later fence redeclared a (non-partial) type from an earlier one, so the earlier fence was dropped as the model's
    /// superseded draft; the remaining fences were joined in order.
    /// </summary>
    Revised,

    /// <summary>No usable C# was found.</summary>
    ExtractFailed,
}
