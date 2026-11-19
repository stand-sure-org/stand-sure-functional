using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        Kind = kind;
        Code = code;
        Message = message;
        Metadata = metadata ?? EmptyMetadata;
        Inner = inner;
        Errors = errors ?? EmptyErrors;
        Exception = exception;
    }

    /// <summary>Gets the error that is the shared placeholder for a default (uninitialized) result.</summary>
    public static Error Uninitialized => UninitializedInstance;

    /// <summary>Gets the category of the failure.</summary>
    public ErrorKind Kind { get; }

    /// <summary>Gets the stable machine-readable code.</summary>
    public string Code { get; }

    /// <summary>Gets the human-readable message.</summary>
    public string Message { get; }

    /// <summary>Gets additional context; never <see langword="null"/>.</summary>
    public IReadOnlyDictionary<string, string> Metadata { get; }

    /// <summary>Gets the error this one wraps, if any.</summary>
    public Error? Inner { get; }

    /// <summary>Gets child errors (for example several validation failures); never <see langword="null"/>.</summary>
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
    /// <param name="code">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="errors">The child errors.</param>
    /// <returns>The error.</returns>
    public static Error Validation(string code, string message, IEnumerable<Error> errors) =>
        Create(ErrorKind.Validation, code, message, null, null, errors is null ? null : errors.ToArray());

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
    public static Error FromException(Exception exception) =>
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
        Create(Kind, code, message, null, this);

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
        if (inner is null)
        {
            FunctionalDiagnostics.RecordFailure(error);
        }
        else
        {
            FunctionalDiagnostics.RecordWrap(error);
        }

        return error;
    }

    private static IReadOnlyDictionary<string, string>? Snapshot(IReadOnlyDictionary<string, string>? metadata) =>
        metadata is null or { Count: 0 }
            ? null
            : new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(metadata.ToArray()));
}
