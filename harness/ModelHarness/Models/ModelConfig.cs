namespace ModelHarness.Models;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

/// <summary>One entry of <c>models.json</c>.</summary>
public sealed record ModelConfig(string Family, string Id, bool Enabled)
{
    public static IReadOnlyList<ModelConfig> LoadEnabled(string json)
    {
        var file = JsonSerializer.Deserialize(json, HarnessJson.Default.ModelConfigFile);
        return file?.Models?.Where(m => m.Enabled).ToList() ?? [];
    }

    public static IReadOnlyList<ModelConfig> LoadEnabledFromFile(string path)
    {
        return LoadEnabled(File.ReadAllText(path));
    }
}
