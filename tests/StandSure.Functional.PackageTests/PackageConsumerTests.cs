namespace StandSure.Functional.PackageTests;

using System;
using System.IO;
using System.Threading.Tasks;

using AwesomeAssertions;

using Xunit;

public sealed class PackageConsumerTests
{
    // No using directive on purpose: the package's buildTransitive props must supply StandSure.Functional.
    private const string ConsumerSource = """
        namespace Consumer;

        public static class Parser
        {
            public static Result<int> Length(string text)
            {
                if (text.Length == 0)
                {
                    return Error.Validation("text.empty", "Text is empty.");
                }

                return text.Length;
            }

            public static Option<int> Maybe(int value) => Option.Some(value);
        }
        """;

    [Fact]
    public async Task GlobalUsing_ResultAvailableWithoutUsingDirective()
    {
        using var consumer = ConsumerProject.Create(ConsumerSource);

        var build = await consumer.BuildAsync(TestContext.Current.CancellationToken);

        build.ExitCode.Should().Be(0, "a file with no using directive should compile against the package. Build output:\n{0}", build.Output);
    }
}
