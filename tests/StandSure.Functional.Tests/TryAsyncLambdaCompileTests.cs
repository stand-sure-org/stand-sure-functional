using System.Linq;

using AwesomeAssertions;

using Xunit;

namespace StandSure.Functional.Tests;

public sealed class TryAsyncLambdaCompileTests
{
    private const string Message = "Use Try.RunAsync(ct => ..., ct) for asynchronous work.";

    [Theory]
    [InlineData("Try.Run(async () => { await Task.Yield(); })")]
    [InlineData("Try.Run(async () => { await Task.Yield(); return 1; })")]
    [InlineData("Try.Run(() => Task.CompletedTask)")]
    [InlineData("Try.Run(() => Task.FromResult(1))")]
    public void Run_WithAsyncDelegate_FailsToCompile_WithRunAsyncHint(string call)
    {
        var errors = CompileProbe.Errors(Wrap(call));

        errors.Should().ContainSingle();
        errors[0].Id.Should().Be("CS0619");
        errors[0].GetMessage(System.Globalization.CultureInfo.InvariantCulture).Should().Contain(Message);
    }

    [Theory]
    [InlineData("Try.Run(() => 42)")]
    [InlineData("Try.Run(() => { })")]
    [InlineData("Try.Run(() => \"x\".Length)")]
    [InlineData("Try.RunAsync(ct => Task.FromResult(1), default)")]
    [InlineData("Try.RunAsync(async ct => { await Task.Delay(1, ct); }, default)")]
    public void Run_WithSyncDelegate_StillCompiles(string call)
    {
        CompileProbe.Errors(Wrap(call)).Should().BeEmpty();
    }

    [Fact]
    public void ObsoleteOverloads_DoNotThrow_WhenReachedByReflection()
    {
        var method = typeof(Try).GetMethods()
            .Single(m => m.Name == "Run" && !m.IsGenericMethod
                                         && m.GetParameters()[0].ParameterType == typeof(System.Func<System.Threading.Tasks.Task>));

        var result = (Result<Unit>)method.Invoke(null, [(System.Func<System.Threading.Tasks.Task>)(() => System.Threading.Tasks.Task.CompletedTask)])!;

        result.TryGetValue(out Error? e).Should().BeTrue();
        e!.Code.Should().Be("try.async_delegate");
    }

    private static string Wrap(string call) => $$"""
                                                 using System.Threading.Tasks;
                                                 using StandSure.Functional;
                                                 public static class Probe
                                                 {
                                                     public static void Run()
                                                     {
                                                         _ = {{call}};
                                                     }
                                                 }
                                                 """;
}
