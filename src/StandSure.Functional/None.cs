namespace StandSure.Functional;

/// <summary>The absence case of an <see cref="Option{T}"/>. It has exactly one instance, <see cref="Instance"/>.</summary>
public sealed class None
{
    /// <summary>The single <see cref="None"/> instance.</summary>
    public static readonly None Instance = new();

    private None()
    {
    }

    /// <inheritdoc />
    public override string ToString() => "None";
}
