using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace StandSure.Functional;

/// <summary>A failure: a kind, a stable machine-readable code, a message and optional context.</summary>
public sealed record Error
{
    // EmptyMetadata and EmptyErrors must be declared before UninitializedInstance: static field initializers run in
    // textual order, and the private constructor reads both.
    private static readonly IReadOnlyDictionary<string, string> EmptyMetadata =
        FrozenDictionary<string, string>.Empty;

    private static readonly IReadOnlyList<Error> EmptyErrors = [];

    private static readonly Error UninitializedInstance = new(
        ErrorKind.Unexpected,
        "result.uninitialized",
        "The result was never initialized (default value).",
        null,
        null,
        null,
        null);

    private Error(
        ErrorKind kind,
        string code,
        string message,
        IReadOnlyDictionary<string, string>? metadata,
        Error? inner,
        IReadOnlyList<Error>? errors,
        Exception? exception)
    {
        this.Kind = kind;
        this.Code = code;
        this.Message = message;
        this.Metadata = metadata ?? EmptyMetadata;
        this.Inner = inner;
        this.Errors = errors ?? EmptyErrors;
        this.Exception = exception;
    }

    // The wire-format constructor: no exception parameter, so the source generator never needs Exception metadata,
    // and it bypasses Create, so deserializing records no failure telemetry. The deserializer hands over a mutable
    // Dictionary and List, so both are copied into the same read-only shapes the factories produce.
    [JsonConstructor]
    private Error(
        ErrorKind kind,
        string code,
        string message,
        IReadOnlyDictionary<string, string>? metadata = null,
        IReadOnlyList<Error>? errors = null,
        Error? inner = null)
        : this(kind, code, message, Snapshot(metadata), inner, errors is null or { Count: 0 } ? null : [.. errors], null)
    {
    }

    /// <summary>Gets the error that is the shared placeholder for a default (uninitialized) result.</summary>
    public static Error Uninitialized => UninitializedInstance;

    /// <summary>Gets the category of the failure.</summary>
    [JsonPropertyName("kind")]
    public ErrorKind Kind { get; }

    /// <summary>Gets the stable machine-readable code.</summary>
    /// <remarks>
    /// Keep codes constant, such as <c>user.not_found</c>. The code is the <c>error.code</c> tag on the
    /// <c>functional.failures</c> counter, so a code built from an id (<c>$"user.{id}.not_found"</c>) makes a new metric
    /// series per id. Put ids and values in <see cref="Metadata"/> instead.
    /// </remarks>
    [JsonPropertyName("code")]
    public string Code { get; }

    /// <summary>Gets the human-readable message.</summary>
    [JsonPropertyName("message")]
    public string Message { get; }

    /// <summary>Gets additional context; never <see langword="null"/>.</summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string> Metadata { get; }

    /// <summary>Gets the error this one wraps, if any.</summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("inner")]
    public Error? Inner { get; }

    /// <summary>Gets child errors (for example several validation failures); never <see langword="null"/>.</summary>
    [JsonPropertyOrder(1)]
    [JsonPropertyName("errors")]
    public IReadOnlyList<Error> Errors { get; }

    /// <summary>Gets the originating exception; kept in-process only and never serialized.</summary>
    [JsonIgnore]
    public Exception? Exception { get; }

    /// <summary>Creates a <see cref="ErrorKind.Validation"/> error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="metadata">Optional context.</param>
    /// <returns>The error.</returns>
    // RS0027: the (code, message, errors) overload is a distinct type-based overload, not a later optional-parameter addition.
#pragma warning disable RS0027
    public static Error Validation(string code, string message, IReadOnlyDictionary<string, string>? metadata = null) =>
        Create(ErrorKind.Validation, code, message, metadata);
#pragma warning restore RS0027

    /// <summary>Creates a <see cref="ErrorKind.Validation"/> parent error that holds several child errors.</summary>
    /// <remarks>
    /// The children were already recorded when they were built, so a parent with at least one child is recorded as a
    /// <c>functional.failure.grouped</c> event and is not counted in <c>functional.failures</c>. With no children it is
    /// an ordinary root failure.
    /// </remarks>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="errors">The child errors.</param>
    /// <returns>The error.</returns>
    public static Error Validation(string code, string message, IEnumerable<Error>? errors) =>
        Create(ErrorKind.Validation, code, message, null, null, errors?.ToArray());

    // The parent built by Combine and Traverse: same telemetry as the public grouping factory, with the kind chosen by
    // the caller from the children.
    internal static Error Group(ErrorKind kind, string code, string message, IReadOnlyList<Error> errors) =>
        Create(kind, code, message, null, null, errors);

    /// <summary>Creates a <see cref="ErrorKind.NotFound"/> error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="metadata">Optional context.</param>
    /// <returns>The error.</returns>
    public static Error NotFound(string code, string message, IReadOnlyDictionary<string, string>? metadata = null) =>
        Create(ErrorKind.NotFound, code, message, metadata);

    /// <summary>Creates a <see cref="ErrorKind.Conflict"/> error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="metadata">Optional context.</param>
    /// <returns>The error.</returns>
    public static Error Conflict(string code, string message, IReadOnlyDictionary<string, string>? metadata = null) =>
        Create(ErrorKind.Conflict, code, message, metadata);

    /// <summary>Creates a <see cref="ErrorKind.Unauthorized"/> error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="metadata">Optional context.</param>
    /// <returns>The error.</returns>
    public static Error Unauthorized(string code, string message, IReadOnlyDictionary<string, string>? metadata = null) =>
        Create(ErrorKind.Unauthorized, code, message, metadata);

    /// <summary>Creates a <see cref="ErrorKind.Forbidden"/> error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="metadata">Optional context.</param>
    /// <returns>The error.</returns>
    public static Error Forbidden(string code, string message, IReadOnlyDictionary<string, string>? metadata = null) =>
        Create(ErrorKind.Forbidden, code, message, metadata);

    /// <summary>Creates a <see cref="ErrorKind.Unavailable"/> error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="metadata">Optional context.</param>
    /// <returns>The error.</returns>
    public static Error Unavailable(string code, string message, IReadOnlyDictionary<string, string>? metadata = null) =>
        Create(ErrorKind.Unavailable, code, message, metadata);

    /// <summary>Creates an <see cref="ErrorKind.Unexpected"/> error.</summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="metadata">Optional context.</param>
    /// <returns>The error.</returns>
    public static Error Unexpected(string code, string message, IReadOnlyDictionary<string, string>? metadata = null) =>
        Create(ErrorKind.Unexpected, code, message, metadata);

    /// <summary>Creates an <see cref="ErrorKind.Unexpected"/> error from an exception.</summary>
    /// <param name="exception">The exception; kept in <see cref="Exception"/>.</param>
    /// <returns>The error, with code <c>exception.unhandled</c>.</returns>
    public static Error FromException(Exception? exception) =>
        exception is null
            ? Create(ErrorKind.Unexpected, "exception.unhandled", "The exception was null.", null)
            : Create(
                ErrorKind.Unexpected,
                "exception.unhandled",
                exception.Message,
                new Dictionary<string, string>
                {
                    ["exception.type"] = exception.GetType().FullName ?? exception.GetType().Name,
                    ["exception.message"] = exception.Message,
                },
                null,
                null,
                exception);

    /// <summary>Wraps this error in a new one of the same <see cref="Kind"/>.</summary>
    /// <param name="code">The new error code.</param>
    /// <param name="message">The new message.</param>
    /// <returns>A new error whose <see cref="Inner"/> is this error.</returns>
    public Error Wrap(string code, string message) =>
        Create(this.Kind, code, message, null, this);

    /// <summary>Determines whether this error or any error in its <see cref="Inner"/> chain has the given code.</summary>
    /// <param name="code">The code to look for.</param>
    /// <returns><see langword="true"/> if found.</returns>
    public bool Is(string code)
    {
        for (var current = this; current is not null; current = current.Inner)
        {
            if (string.Equals(current.Code, code, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static Error Create(
        ErrorKind kind,
        string code,
        string message,
        IReadOnlyDictionary<string, string>? metadata,
        Error? inner = null,
        IReadOnlyList<Error>? errors = null,
        Exception? exception = null)
    {
        var error = new Error(kind, code, message, Snapshot(metadata), inner, errors, exception);

        if (inner is not null)
        {
            FunctionalDiagnostics.RecordWrap(error);
        }
        else if (error.Errors.Count > 0)
        {
            // A parent that only groups children: the children were counted when they were built.
            FunctionalDiagnostics.RecordGroup(error);
        }
        else
        {
            FunctionalDiagnostics.RecordFailure(error);
        }

        return error;
    }

    private static IReadOnlyDictionary<string, string>? Snapshot(IReadOnlyDictionary<string, string>? metadata) =>
        metadata is null or { Count: 0 }
            ? null
            : new ReadOnlyDictionary<string, string>(new Dictionary<string, string>([.. metadata]));

    /// <summary>Compares two errors by their contents.</summary>
    /// <remarks>
    /// <see cref="Kind"/>, <see cref="Code"/> and <see cref="Message"/> (ordinal); <see cref="Metadata"/> as a set of
    /// key/value pairs, in any order; <see cref="Inner"/> recursively; <see cref="Errors"/> in order; and
    /// <see cref="Exception"/> by reference.
    /// </remarks>
    /// <param name="other">The error to compare with.</param>
    /// <returns><see langword="true"/> when both describe the same failure.</returns>
    public bool Equals(Error? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null)
        {
            return false;
        }

        return this.Kind == other.Kind
               && string.Equals(this.Code, other.Code, StringComparison.Ordinal)
               && string.Equals(this.Message, other.Message, StringComparison.Ordinal)
               && ReferenceEquals(this.Exception, other.Exception)
               && SameMetadata(this.Metadata, other.Metadata)
               && (this.Inner is null ? other.Inner is null : this.Inner.Equals(other.Inner))
               && this.Errors.SequenceEqual(other.Errors);
    }

    /// <summary>Gets a hash code that agrees with <see cref="Equals(Error)"/>.</summary>
    /// <returns>The hash code.</returns>
    // HashCode.Combine hashes through EqualityComparer<T>.Default: ordinal for strings and null-safe, which matters
    // because nullable annotations do not stop a null Code or Message at runtime.
    public override int GetHashCode() =>
        HashCode.Combine(this.Kind, this.Code, this.Message, this.Metadata.Count, this.Errors.Count, this.Inner);

    private static bool SameMetadata(IReadOnlyDictionary<string, string> left, IReadOnlyDictionary<string, string> right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left.Count != right.Count)
        {
            return false;
        }

        foreach (var (key, value) in left)
        {
            if (!right.TryGetValue(key, out var other) || !string.Equals(value, other, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    // ToString() is "Error { <members> }". The message and metadata values follow the EmitErrorDetails switch, as
    // telemetry does: a logged {error} leaves the process just as an exported span does. The exception's type is
    // printed, never the exception itself (its message and stack).
    private bool PrintMembers(StringBuilder builder)
    {
        var details = FunctionalDiagnostics.DetailsEnabled;

        builder.Append("Kind = ").Append(this.Kind);
        builder.Append(", Code = ").Append(this.Code);

        if (details)
        {
            builder.Append(", Message = ").Append(this.Message);
        }

        builder.Append(", Metadata = [");
        var first = true;
        foreach (var key in this.Metadata.Keys.Order(StringComparer.Ordinal))
        {
            builder.Append(first ? string.Empty : ", ").Append(key);
            if (details)
            {
                builder.Append('=').Append(this.Metadata[key]);
            }

            first = false;
        }

        builder.Append(']');
        builder.Append(", Errors = ").Append(this.Errors.Count);

        if (this.Inner is not null)
        {
            builder.Append(", Inner = ").Append(this.Inner);
        }

        if (this.Exception is not null)
        {
            builder.Append(", Exception = ").Append(this.Exception.GetType().FullName);
        }

        return true;
    }
}
