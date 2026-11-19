public interface IConfigSource
{
    /// <summary>Returns the raw configured value for the key, or null when the key is not set.</summary>
    string? GetValue(string key);
}
