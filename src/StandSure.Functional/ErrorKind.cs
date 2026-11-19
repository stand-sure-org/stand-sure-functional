namespace StandSure.Functional;

/// <summary>The category of a failure, independent of its specific code.</summary>
public enum ErrorKind
{
    /// <summary>The input was invalid.</summary>
    Validation,

    /// <summary>The requested thing does not exist.</summary>
    NotFound,

    /// <summary>The request conflicts with current state.</summary>
    Conflict,

    /// <summary>The caller is not authenticated.</summary>
    Unauthorized,

    /// <summary>The caller is authenticated but not allowed.</summary>
    Forbidden,

    /// <summary>A dependency is temporarily unavailable.</summary>
    Unavailable,

    /// <summary>An unexpected failure.</summary>
    Unexpected,
}
