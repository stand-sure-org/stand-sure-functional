using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using StandSure.Functional.Json;

namespace StandSure.Functional;

/// <summary>
/// The outcome of an operation that can fail: either a success holding a <typeparamref name="T"/>
/// or a failure holding an <see cref="Error"/>.
/// </summary>
/// <typeparam name="T">The success value type.</typeparam>
/// <remarks>
/// <para>
/// A non-boxing C# union: <c>switch</c> on it directly, for example
/// <c>r switch { Error e =&gt; …, T value =&gt; … }</c>. For a non-nullable <typeparamref name="T"/> other than
/// <see cref="Error"/>, that switch is exhaustive without a discard, and leaving out a case is error CS8509.
/// A nullable reference <typeparamref name="T"/> (for example <c>string?</c>) may also need a <c>null</c> arm.
/// </para>
/// <para>
/// When <typeparamref name="T"/> is <see cref="Error"/>, a <c>switch</c> cannot observe success: the
/// <c>Error e</c> pattern binds only to the failure case, so a success never matches it, yet the compiler
/// still treats the switch as exhaustive. Consume a <c>Result&lt;Error&gt;</c> through <see cref="IsSuccess"/>,
/// <see cref="Deconstruct"/> or <see cref="Value"/> instead. Its two <c>TryGetValue</c> overloads collapse to
/// one signature, <c>TryGetValue(out Error?)</c>, and the failure overload wins: it returns
/// <see langword="false"/> on a success.
/// </para>
/// <para>
/// <c>default(Result&lt;T&gt;)</c>, such as an element of a new array or an unset field, is a failure
/// whose error is <see cref="Error.Uninitialized"/>. No member throws.
/// </para>
/// <para>
/// Success or failure comes from the stored state, never from the value's runtime type.
/// <c>Result.Success&lt;object&gt;(someError)</c> is a success. <c>Result&lt;object&gt; r = someError</c>
/// is a failure, because the more specific conversion wins. The implicit conversion from
/// <see cref="Error"/> to <c>Result&lt;Error&gt;</c> is ambiguous and fails to compile (CS0457).
/// Use <see cref="Result.Success{T}(T)"/> or <see cref="Result.Failure{T}(Error)"/> to say which one you mean.
/// </para>
/// </remarks>
[Union]
[JsonConverter(typeof(ResultJsonConverterFactory))]
[StackTraceHidden]
[DebuggerDisplay("{DebugView,nq}")]
public readonly partial struct Result<T> : IUnion
{
    private const byte TagDefault = 0;
    private const byte TagOk = 1;
    private const byte TagFail = 2;

    private readonly T _value;
    private readonly Error? _error;
    private readonly byte _tag;

    /// <summary>Initializes a new instance of the <see cref="Result{T}"/> struct as a success.</summary>
    /// <param name="value">The success value; <see langword="null"/> is allowed and is still a success.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result(T value)
    {
        _value = value;
        _error = null;
        _tag = TagOk;
    }

    /// <summary>Initializes a new instance of the <see cref="Result{T}"/> struct as a failure.</summary>
    /// <param name="error">The error. A <see langword="null"/> error is replaced with <see cref="Error.Uninitialized"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result(Error error)
    {
        _value = default!;
        _error = error ?? Error.Uninitialized;
        _tag = TagFail;
    }

    /// <summary>Gets a value indicating whether this result is a success.</summary>
    public bool IsSuccess
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tag == TagOk;
    }

    /// <summary>Gets a value indicating whether this result is a failure, including the <c>default</c> value.</summary>
    public bool IsFailure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tag != TagOk;
    }

    /// <summary>Gets a value indicating whether the union holds a case. Always <see langword="true"/>,
    /// because the <c>default</c> value is the <see cref="Error.Uninitialized"/> failure.</summary>
    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    /// <summary>Gets the current case as an object: the success value, or the <see cref="Error"/>.
    /// Never throws. Boxes a value-type <typeparamref name="T"/>, so prefer <c>switch</c> or <c>TryGetValue</c>.</summary>
    public object? Value => _tag == TagOk ? _value : FailureError;

    private Error FailureError
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _tag == TagDefault ? Error.Uninitialized : _error ?? Error.Uninitialized;
    }

    private string DebugView => _tag == TagOk ? $"Ok({_value})" : $"Fail({FailureError.Code})";

    /// <summary>Gets the error if this result is a failure.</summary>
    /// <param name="error">The error (<see cref="Error.Uninitialized"/> for <c>default</c>); <see langword="null"/> on success.</param>
    /// <returns><see langword="true"/> if this result is a failure.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue([NotNullWhen(true)] out Error? error)
    {
        if (_tag == TagOk)
        {
            error = null;
            return false;
        }

        error = FailureError;
        return true;
    }

    /// <summary>Gets the success value if this result is a success.</summary>
    /// <param name="value">The value; <c>default</c> on failure.</param>
    /// <returns><see langword="true"/> if this result is a success.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (_tag == TagOk)
        {
            value = _value;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>Deconstructs Go-style: <c>var (value, error) = result;</c> then check <c>error</c> first.</summary>
    /// <param name="value">The value on success; <c>default</c> on failure.</param>
    /// <param name="error"><see langword="null"/> on success; the error on failure
    /// (<see cref="Error.Uninitialized"/> for <c>default</c>).</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out T? value, out Error? error)
    {
        if (_tag == TagOk)
        {
            value = _value;
            error = null;
            return;
        }

        value = default;
        error = FailureError;
    }
}
