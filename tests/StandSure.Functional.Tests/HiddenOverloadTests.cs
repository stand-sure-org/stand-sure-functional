using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

/// <summary>
/// Review finding 6: <c>Ensure(predicate)</c> with no error turns the predicate's source text into the message
/// (<c>"u =&gt; u.Age &gt;= 13"</c>), which can reach users through an HTTP mapping. It stays callable but is hidden from
/// IntelliSense, like <c>Ok</c>/<c>Fail</c>, so completion offers the overloads that take an error.
/// </summary>
public sealed class HiddenOverloadTests
{
    [Fact]
    public void EnsureWithExpressionMessage_IsHiddenFromIntelliSense()
    {
        var method = typeof(Result<int>).GetMethods()
            .Single(m => m.Name == "Ensure" && m.GetParameters() is [_, { ParameterType: var p }] && p == typeof(string));

        var attribute = method.GetCustomAttribute<EditorBrowsableAttribute>();

        attribute.Should().NotBeNull();
        attribute!.State.Should().Be(EditorBrowsableState.Never);
    }

    [Fact]
    public void EnsureOverloadsThatTakeAnError_StayVisible()
    {
        var visible = typeof(Result<int>).GetMethods()
            .Where(m => m.Name == "Ensure" && m.GetCustomAttribute<EditorBrowsableAttribute>() is null)
            .Select(m => m.GetParameters()[1].ParameterType)
            .ToArray();

        visible.Should().BeEquivalentTo([typeof(Error), typeof(Func<int, Error>)]);
    }
}
