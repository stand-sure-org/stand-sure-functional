using System;

namespace StandSure.Functional.Tests;

/// <summary>An exception whose <see cref="Message"/> is null, as some third-party exceptions return.</summary>
internal sealed class NullMessageException : Exception
{
    public override string Message => null!;
}
