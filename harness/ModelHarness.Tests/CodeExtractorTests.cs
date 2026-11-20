namespace ModelHarness.Tests;

using System;

using AwesomeAssertions;

using ModelHarness.Compile;

public sealed class CodeExtractorTests
{
    [Fact]
    public void NoFence_InvalidCSharp_ExtractFailed()
    {
        var result = CodeExtractor.Extract("Sure! Here is how I would do it, you should validate the input first.");

        result.Status.Should().Be(ExtractStatus.ExtractFailed);
        result.Code.Should().BeNull();
    }

    [Fact]
    public void NoFence_ValidCSharp_Single()
    {
        const string code = "public class Solution { public int Run() => 1; }";

        var result = CodeExtractor.Extract(code);

        result.Status.Should().Be(ExtractStatus.Single);
        result.Code.Should().Be(code);
    }

    [Fact]
    public void EmptyOrWhitespace_ExtractFailed()
    {
        CodeExtractor.Extract("  \n ").Status.Should().Be(ExtractStatus.ExtractFailed);
    }

    [Theory]
    [InlineData("csharp")]
    [InlineData("cs")]
    [InlineData("c#")]
    [InlineData("CSharp")]
    [InlineData("")]
    public void SingleFence_AcceptedLabels_Single(string label)
    {
        var text = $"Here you go:\n```{label}\npublic class Solution {{ }}\n```\nDone.";

        var result = CodeExtractor.Extract(text);

        result.Status.Should().Be(ExtractStatus.Single);
        result.Code.Should().Be("public class Solution { }");
    }

    [Fact]
    public void OtherLanguageFences_AreIgnored()
    {
        var text = "```json\n{\"a\":1}\n```\n```csharp\npublic class Solution { }\n```\n```bash\ndotnet run\n```";

        var result = CodeExtractor.Extract(text);

        result.Status.Should().Be(ExtractStatus.Single);
        result.Code.Should().Be("public class Solution { }");
    }

    [Fact]
    public void OnlyOtherLanguageFences_ExtractFailed()
    {
        CodeExtractor.Extract("```xml\n<a/>\n```").Status.Should().Be(ExtractStatus.ExtractFailed);
    }

    [Fact]
    public void TwoFences_Concatenated()
    {
        var text = "```csharp\npublic class A { }\n```\ntext\n```cs\npublic class B { }\n```";

        var result = CodeExtractor.Extract(text);

        result.Status.Should().Be(ExtractStatus.Concatenated);
        result.Code.Should().Contain("class A").And.Contain("class B");
        result.Code!.IndexOf("class A", StringComparison.Ordinal).Should().BeLessThan(result.Code.IndexOf("class B", StringComparison.Ordinal));
    }

    [Fact]
    public void Concatenated_HoistsUsingsAndDeduplicates()
    {
        var text = "```csharp\nusing System;\npublic class A { }\n```\n```csharp\nusing System;\nusing System.Linq;\npublic class B { }\n```";

        var result = CodeExtractor.Extract(text);

        var code = result.Code!;
        code.Split("using System;").Should().HaveCount(2);
        code.IndexOf("using System.Linq;", StringComparison.Ordinal).Should().BeLessThan(code.IndexOf("class A", StringComparison.Ordinal));
        code.IndexOf("using System;", StringComparison.Ordinal).Should().BeLessThan(code.IndexOf("class A", StringComparison.Ordinal));
    }

    [Fact]
    public void UnterminatedFinalFence_TakesContentToEnd()
    {
        var result = CodeExtractor.Extract("```csharp\npublic class Solution { }\n");

        result.Status.Should().Be(ExtractStatus.Single);
        result.Code.Should().Contain("class Solution");
    }

    [Fact]
    public void CrLfLineEndings_AreHandled()
    {
        var result = CodeExtractor.Extract("```csharp\r\npublic class Solution { }\r\n```\r\n");

        result.Status.Should().Be(ExtractStatus.Single);
        result.Code.Should().Contain("class Solution");
    }
}
