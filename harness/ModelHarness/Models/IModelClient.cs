namespace ModelHarness.Models;

using System.Threading;
using System.Threading.Tasks;

/// <summary>A model provider. Failures are reported in <see cref="ModelResponse.Error"/>; only cancellation throws.</summary>
public interface IModelClient
{
    string Family { get; }

    Task<ModelResponse> CompleteAsync(ModelRequest request, CancellationToken ct);
}
