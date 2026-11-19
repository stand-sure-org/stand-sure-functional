using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using StandSure.Functional.Json;

namespace StandSure.Functional;

/// <summary>
/// An optional value: either some <typeparamref name="T"/> or <see cref="StandSure.Functional.None"/>.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <remarks>
/// <para>
/// A non-boxing C# union: <c>switch</c> on it directly, for example
/// <c>o switch { None =&gt; …, T value =&gt; … }</c>. For a non-nullable <typeparamref name="T"/> other than
/// <see cref="StandSure.Functional.None"/>, that switch is exhaustive without a discard, and leaving out a case
/// is error CS8509.
/// </para>
/// <para>
/// When <typeparamref name="T"/> is <see cref="StandSure.Functional.None"/>, a <c>switch</c> cannot observe
/// some: the <c>None</c> pattern binds only to the absent case. Use <see cref="IsSome"/> instead.
/// </para>
/// <para>
/// A <see langword="null"/> <typeparamref name="T"/> (including a null <c>int?</c>) becomes none.
/// <c>default(Option&lt;T&gt;)</c>, such as an element of a new array or an unset field, is none.
/// No member throws.
/// </para>
/// </remarks>
[Union]
[JsonConverter(typeof(OptionJsonConverterFactory))]
[StackTraceHidden]
[DebuggerDisplay("{DebugView,nq}")]
public readonly partial struct Option<T> : IUnion
{
    private readonly T _value;
    private readonly bool _isSome;

    /// <summary>Initializes a new instance of the <see cref="Option{T}"/> struct holding a value.</summary>
    /// <param name="value">The value; <see langword="null"/> produces none.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option([AllowNull] T value)
    {
        if (default(T) is null && value is null)
        {
            _value = default!;
            _isSome = false;
            return;
        }

        _value = value!;
        _isSome = true;
    }

    /// <summary>Initializes a new instance of the <see cref="Option{T}"/> struct as none.</summary>
    /// <param name="none">Ignored; the only <see cref="StandSure.Functional.None"/> value is the absence case.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option(global::StandSure.Functional.None none)
    {
        _value = default!;
        _isSome = false;
    }

    /// <summary>Gets the none option.</summary>
    public static Option<T> None
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    /// <summary>Gets a value indicating whether this option holds a value.</summary>
    public bool IsSome
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isSome;
    }

    /// <summary>Gets a value indicating whether this option is none, including the <c>default</c> value.</summary>
    public bool IsNone
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !_isSome;
    }

    /// <summary>Gets a value indicating whether the union holds a case. Always <see langword="true"/>,
    /// because the <c>default</c> value is none.</summary>
    public bool HasValue
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => true;
    }

    /// <summary>Gets the current case as an object: the value, or <see cref="StandSure.Functional.None.Instance"/>
    /// when absent. Never throws. Boxes a value-type <typeparamref name="T"/>, so prefer <c>switch</c> or
    /// <c>TryGetValue</c>.</summary>
    public object? Value => _isSome ? _value : global::StandSure.Functional.None.Instance;

    private string DebugView => _isSome ? $"Some({_value})" : "None";

    /// <summary>Gets the none case if this option is none.</summary>
    /// <param name="none"><see cref="StandSure.Functional.None.Instance"/> when none; <see langword="null"/> otherwise.</param>
    /// <returns><see langword="true"/> if this option is none.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue([NotNullWhen(true)] out global::StandSure.Functional.None? none)
    {
        if (_isSome)
        {
            none = null;
            return false;
        }

        none = global::StandSure.Functional.None.Instance;
        return true;
    }

    /// <summary>Gets the value if this option is some.</summary>
    /// <param name="value">The value; <c>default</c> when none.</param>
    /// <returns><see langword="true"/> if this option is some.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (_isSome)
        {
            value = _value;
            return true;
        }

        value = default;
        return false;
    }
}
